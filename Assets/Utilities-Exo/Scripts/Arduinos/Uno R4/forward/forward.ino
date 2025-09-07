#include <WiFiS3.h>

/************ NETWORK CONFIG ************/
const char* SSID     = "parter";
const char* PASSWORD = "5b2ec2d9";

IPAddress ip(192, 168, 1, 102);
IPAddress gateway(192, 168, 1, 1);
IPAddress subnet(255, 255, 255, 0);
IPAddress dns(192, 168, 1, 1);

/************ SERVER CONFIG *************/
const uint16_t TCP_PORT = 12345;
WiFiServer server(TCP_PORT);

/************ UART to UNO classics ******/
const unsigned long LINK_BAUD = 9600;   // must match each UNO's SoftwareSerial
const unsigned long USB_BAUD  = 115200;
const int REF_MIN = 0;
const int REF_MAX = 1023;

/************ BUFFERS ************/
static char lineBuf[64];
static uint8_t lineLen = 0;

/************ SEND HELPERS (Serial1 pins 0/1) ************/
void sendClipped(int &v) { if (v < REF_MIN) v = REF_MIN; if (v > REF_MAX) v = REF_MAX; }

void sendToU1(int v){ sendClipped(v); Serial.print("R4 -> U1: "); Serial.println(v); Serial1.print("U1 REF "); Serial1.println(v); }
void sendToU2(int v){ sendClipped(v); Serial.print("R4 -> U2: "); Serial.println(v); Serial1.print("U2 REF "); Serial1.println(v); }
void sendToU3(int v){ sendClipped(v); Serial.print("R4 -> U3: "); Serial.println(v); Serial1.print("U3 REF "); Serial1.println(v); }
void sendToU4(int v){ sendClipped(v); Serial.print("R4 -> U4: "); Serial.println(v); Serial1.print("U4 REF "); Serial1.println(v); }
void sendToAll(int v){ sendClipped(v); Serial.print("R4 -> ALL: "); Serial.println(v); Serial1.print("ALL REF "); Serial1.println(v); }

/************ UTILS ************/
const char* ltrim(const char* s){ while(*s==' '||*s=='\t') ++s; return s; }
bool isNumStart(char c){ return (c=='+'||c=='-'||(c>='0'&&c<='9')); }

/************ ROUTER ************/
const char* routeFromUnity(const char* rawLine, char* outReply, size_t outLen) {
  // local copy
  char buf[64]; size_t n = strlen(rawLine); if (n >= sizeof(buf)) n = sizeof(buf)-1;
  memcpy(buf, rawLine, n); buf[n] = '\0';

  const char* s = ltrim(buf);
  if (*s == '\0'){ snprintf(outReply, outLen, "ERR"); return outReply; }

  // "ID value"
  char id[8]={0}; int valOnly=0;
  if (sscanf(s, "%7s %d", id, &valOnly) == 2) {
    sendClipped(valOnly);
    if (!strcasecmp(id,"U1")) { sendToU1(valOnly); snprintf(outReply,outLen,"ACK U1 %d",valOnly); return outReply; }
    if (!strcasecmp(id,"U2")) { sendToU2(valOnly); snprintf(outReply,outLen,"ACK U2 %d",valOnly); return outReply; }
    if (!strcasecmp(id,"U3")) { sendToU3(valOnly); snprintf(outReply,outLen,"ACK U3 %d",valOnly); return outReply; }
    if (!strcasecmp(id,"U4")) { sendToU4(valOnly); snprintf(outReply,outLen,"ACK U4 %d",valOnly); return outReply; }
    if (!strcasecmp(id,"ALL")){ sendToAll(valOnly); snprintf(outReply,outLen,"ACK ALL %d",valOnly); return outReply; }
  }

  snprintf(outReply, outLen, "ERR");
  return outReply;
}

/************ SETUP ************/
void setup() {
  Serial.begin(USB_BAUD);
  unsigned long t0 = millis(); while(!Serial && millis()-t0<3000) {}

  Serial1.begin(LINK_BAUD); // UART to UNOs

  Serial.println("R4: starting WiFi with static IP...");
  WiFi.config(ip, dns, gateway, subnet);
  WiFi.begin(SSID, PASSWORD);
  Serial.print("R4: connecting to WiFi "); Serial.print(SSID); Serial.print(" ...");
  int tries=0; while(WiFi.status()!=WL_CONNECTED && tries<60){ delay(500); Serial.print('.'); tries++; }
  Serial.println();
  if (WiFi.status()!=WL_CONNECTED) {
    Serial.println("R4: WiFi failed. Check SSID/PASS or IP settings.");
  } else {
    Serial.print("R4: WiFi connected. IP = "); Serial.println(WiFi.localIP());
  }

  server.begin();
  Serial.print("R4: TCP server listening on "); Serial.print(WiFi.localIP()); Serial.print(':'); Serial.println(TCP_PORT);
}

/************ LOOP ************/
void loop() {
  WiFiClient client = server.available();
  if (client) {
    if (!client.connected()) { client.stop(); }
    else {
      while (client.available()) {
        char c = (char)client.read();
        if (c=='\r' || c=='\n') {
          if (lineLen>0) {
            lineBuf[lineLen]='\0';
            char reply[32];
            routeFromUnity(lineBuf, reply, sizeof(reply));
            client.println(reply);
            Serial.print("Unity> '"); Serial.print(lineBuf); Serial.print("' -> "); Serial.println(reply);
            lineLen=0;
          }
        } else {
          if (lineLen < sizeof(lineBuf)-1) lineBuf[lineLen++]=c; else lineLen=0;
        }
      }
    }
  }

  // Optional USB <-> Serial1 bridge
  while (Serial.available()) Serial1.write(Serial.read());
  while (Serial1.available()) Serial.write(Serial1.read());
}
