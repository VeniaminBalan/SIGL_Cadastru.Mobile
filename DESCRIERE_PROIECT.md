# SIGL Cadastru Mobile

**Autor:** Balan Veniamin
**Platforma:** .NET MAUI (Android, iOS)  
**Versiune:** 1.0  

---

## 1. Descriere scurtă a cerințelor aplicației și problema ce o rezolvă

SIGL Cadastru Mobile este o aplicație mobilă destinată gestionării și monitorizării cerințelor cadastrale pe dispozitive mobile. Aplicația rezolvă problema accesului limitat la informațiile cadastrale în teren și necesitatea de a gestiona eficient fluxul de lucru pentru lucrările cadastrale.

### Cerințe principale:
- **Autentificare securizată** prin Keycloak/OIDC cu suport pentru refresh token și logout
- **Gestionarea cerințelor cadastrale**: vizualizare, filtrare, căutare și sortare a cerințelor
- **Detalii complete ale cerințelor**: informații despre clienți, lucrări cadastrale, documente, stări și timeline
- **Gestionarea clienților**: acces la baza de date de clienți și informațiile acestora
- **Profil utilizator**: vizualizare și gestionare informații personale
- **Notificări push** prin Firebase Cloud Messaging pentru actualizări în timp real
- **Tracking dispozitiv** cu identificare unică și înregistrare în sistem
- **Suport offline** prin stocare securizată locală (SecureStorage)
- **Interfață intuitivă** construită cu C# Markup (zero XAML) pentru performanță maximă

### Problema rezolvată:
Aplicația elimină necesitatea accesului constant la un computer desktop pentru vizualizarea și gestionarea cerințelor cadastrale. Profesioniștii din domeniul cadastral pot:
- Accesa rapid informațiile despre cereri direct din teren
- Primi notificări instant despre schimbări de stare
- Vizualiza documentele asociate cerințelor
- Gestiona relațiile cu clienții mobil
- Menține sincronizarea automată între dispozitiv și server

---

## 2. Identificarea grupului țintă de utilizatori

### Utilizatori primari:
1. **Ingineri cadastrali** - accesează cerințele în teren, verifică documentația, actualizează stări
2. **Responsabili de proiecte** - monitorizează progresul cerințelor, gestionează termene
3. **Personal de recepție** - înregistrează cereri noi, verifică documentația clienților
4. **Manageri de birouri cadastrale** - supraveghere generală, raportare, analitică

### Caracteristici utilizatori:
- **Vârstă:** 25-55 ani
- **Experiență tehnică:** medie-avansată în utilizarea smartphone-urilor
- **Context de utilizare:** mobilitate constantă între birou și teren
- **Nevoi:** acces rapid la informații, notificări în timp real, interfață simplă

### Scenarii de utilizare:
- Inginerul verifică pe teren datele cadastrale și actualizează starea cererii
- Responsabilul primește notificare despre o cerere cu termen aproape expirat
- Personalul de recepție verifică istoricul unui client înainte de a accepta o cerere nouă
- Managerul monitorizează progresul tuturor cerințelor active din organizație

---

## 3. Descrierea competiției (aplicații similare)

### 3.1 **GeoWorks Mobile** (SUA/Canada)
- **Descriere:** Aplicație pentru gestionarea lucrărilor de topografie și cadastru
- **Puncte forte:** Integrare cu echipamente GPS, capturare coordonate în teren
- **Puncte slabe:** Interfață complexă, licențiere foarte scumpă ($500+/utilizator/an)
- **Diferențiator SIGL:** Interfață mai simplă, focus pe workflow-ul cerințelor, nu pe măsurători

### 3.2 **Cadastral Manager Pro** (Europa)
- **Descriere:** Sistem ERP cadastral cu modul mobile limitat
- **Puncte forte:** Funcționalități comprehensive pentru birou, raportare avansată
- **Puncte slabe:** App mobile este doar viewer, fără capacități de editare, offline limitat
- **Diferențiator SIGL:** Aplicație nativă MAUI cu performanță superioară, notificări push integrate, funcționalitate completă mobile

---

## 4. Descrierea use-case-ului principal

### Use-case: **Monitorizarea și actualizarea unei cerințe cadastrale**

#### Actori:
- **Actor principal:** Inginer cadastral
- **Actori secundari:** Sistem backend SIGL, Serviciu de notificări FCM

#### Precondiții:
- Utilizatorul este autentificat în aplicație
- Dispozitivul are conexiune internet (pentru sincronizare)
- Utilizatorul are permisiuni pentru vizualizarea și editarea cerințelor

#### Flux principal:

1. **Pornire aplicație și autentificare**
   - Utilizatorul deschide aplicația SIGL Cadastru Mobile
   - Dacă nu e autentificat, apasă butonul "Login"
   - Sistemul deschide browser-ul Keycloak pentru autentificare
   - După autentificare, utilizatorul e redirecționat la pagina principală

2. **Vizualizare listă cerințe**
   - Aplicația afișează lista Top 10 cerințe active
   - Utilizatorul vede: număr cerere, număr cadastral, client, responsabil, termenele, starea curentă
   - Utilizatorul poate filtra după stare (Emisă, Respinsă, La recepție, În progres)
   - Utilizatorul poate căuta după text (număr, client, cadastral)
   - Utilizatorul poate sorta după diverse criterii (dată, stare, client)

3. **Scroll infinit pentru mai multe cerințe**
   - La scroll jos, aplicația încarcă automat următoarele 10 cerințe
   - Indicator de loading afișează progresul încărcării

4. **Selectare cerință pentru detalii**
   - Utilizatorul apasă pe o cerință din listă
   - Aplicația navighează la pagina de detalii cerință

5. **Vizualizare detalii complete**
   - Afișează informații client (nume, email, telefon, adresă)
   - Afișează responsabil și executant
   - Afișează metadata: stare curentă, termene, număr oficial
   - Afișează lista lucrărilor cadastrale cu prețuri
   - Afișează prețul total
   - Afișează documentele atașate
   - Afișează timeline complet cu toate schimbările de stare

6. **Primire notificare în timp real** (flux alternativ asincron)
   - În timp ce utilizatorul lucrează, primește o notificare FCM
   - Notificarea indică o schimbare de stare pentru o cerință
   - Utilizatorul apasă pe notificare
   - Aplicația deschide direct cerința respectivă cu datele actualizate

7. **Logout și securitate**
   - Utilizatorul accesează pagina de profil
   - Apasă "Logout"
   - Aplicația șterge tokenurile de autentificare
   - Dispozitivul este deinregistrat din sistem
   - Utilizatorul este redirecționat la pagina de login

#### Postcondiții:
- Utilizatorul a accesat și vizualizat informațiile necesare
- Toate acțiunile sunt înregistrate în system logs
- Dispozitivul rămâne înregistrat pentru notificări (dacă utilizatorul nu a făcut logout)

#### Fluxuri alternative:

**A1. Conexiune pierdută în timpul utilizării**
- Aplicația afișează mesaj de eroare
- Datele deja încărcate rămân vizibile (cache local)
- La reconectare, aplicația sincronizează automat datele

**A2. Token expirat**
- Aplicația detectează automat expirarea token-ului
- Încearcă refresh automat folosind refresh token
- Dacă refresh eșuează, redirecționează utilizatorul la login

**A3. Notificare primită când aplicația e închisă**
- Notificarea apare în notification tray-ul sistemului
- La deschiderea aplicației prin notificare, se navigă direct la cerința respectivă

---

## 5. Identificarea modului posibil de monetizare

### Model de business: **B2B SaaS - Software as a Service pentru companii**

#### 1. **Licențiere per utilizator/lună** (Model principal)
- **Tier Basic:** 29 EUR/utilizator/lună
  - Până la 100 cerințe active
  - 2GB stocare documente
  - Suport email
  
- **Tier Professional:** 49 EUR/utilizator/lună
  - Cerințe nelimitate
  - 10GB stocare
  - Notificări push avansate
  - Suport prioritar
  
- **Tier Enterprise:** 79 EUR/utilizator/lună (minim 10 utilizatori)
  - Toate funcționalitățile Professional
  - Stocare nelimitată
  - Integrare AD/LDAP custom
  - Single Sign-On (SSO)
  - SLA 99.9% uptime
  - Account manager dedicat

#### 2. **White-label pentru parteneri**
- Licențiere către firme mari de consultanță cadastrală
- 30% din revenue-ul generat de clientul final
- Partenerul oferă suport nivel 1, SIGL oferă nivel 2-3

### Proiecție financiară (an 1):
- **Target:** 20 organizații × 8 utilizatori medie = 160 utilizatori activi
- **Average Revenue Per User (ARPU):** 45 EUR/lună
- **MRR (Monthly Recurring Revenue):** 7.200 EUR
- **ARR (Annual Recurring Revenue):** 86.400 EUR
- **+ Servicii adiționale și implementări:** ~25.000 EUR/an
- **Total Year 1:** ~110.000 EUR

---

## 6. Bibliografie și surse folosite

### 6.1 Framework și tehnologii de bază
1. **Microsoft .NET MAUI Documentation**
   - Sursa: https://learn.microsoft.com/en-us/dotnet/maui/
   - Utilizare: Arhitectură de bază, navigare Shell, platform-specific code
   - Realizat în plus: Strict MVVM cu zero XAML pentru UI, doar C# Markup

2. **CommunityToolkit.Mvvm**
   - Sursa: https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/
   - Utilizare: Source generators pentru `[ObservableProperty]` și `[RelayCommand]`
   - Realizat în plus: Structură DI centralizată cu extension methods în `DI.cs`

3. **CommunityToolkit.Maui.Markup**
   - Sursa: https://learn.microsoft.com/en-us/dotnet/communitytoolkit/maui/markup/markup
   - Utilizare: C# Fluent API pentru construirea UI
   - Realizat în plus: Pattern standardizat pentru toate pagini, zero XAML (except AppShell)

### 6.2 Autentificare și securitate
4. **Duende IdentityModel.OidcClient**
   - Sursa: https://docs.duendesoftware.com/identitymodel/
   - Tutorial: https://github.com/IdentityModel/IdentityModel.OidcClient.Samples
   - Utilizare: Integrare OIDC cu Keycloak
   - Realizat în plus: 
     - Custom `WebAuthenticatorBrowser` pentru MAUI's WebAuthenticator
     - Wrapper `KeycloakAuthService` cu token persistence în SecureStorage
     - Auto-refresh token logic
     - `AuthenticatedHttpMessageHandler` pentru injectare automată Bearer token în toate request-urile

5. **Keycloak Authentication**
   - Sursa: https://www.keycloak.org/docs/latest/securing_apps/
   - Utilizare: Configurare realm, client, scopes
   - Realizat în plus: Custom URL scheme `sigl.mobile://callback`, integrare completă cu MAUI lifecycle

### 6.3 Notificări push și device management
6. **Plugin.Firebase for .NET MAUI**
   - Sursa: https://github.com/TobiasBuchholz/Plugin.Firebase
   - Utilizare: Firebase Cloud Messaging pentru push notifications
   - Realizat în plus:
     - `DeviceManager` service pentru tracking unic device ID (persistență în SecureStorage)
     - `DeviceHeaderHandler` - custom HTTP message handler pentru injectare automată device ID în headers
     - Auto-register device la login, auto-deregister la logout
     - `NotificationService` pentru procesare notificări în foreground/background

7. **Firebase Cloud Messaging Documentation**
   - Sursa: https://firebase.google.com/docs/cloud-messaging
   - Utilizare: Configurare google-services.json, FCM token handling
   - Realizat în plus: Integrare bidirectională device ↔ backend API pentru tracking sesiuni

### 6.4 Arhitectură și design patterns
8. **.NET MAUI Community Samples**
   - Sursa: https://github.com/dotnet/maui-samples
   - Utilizare: Exemple pentru MVVM navigation, Shell routing
   - Realizat în plus: 
     - Structură modulară strict organizată (Models/Services/ViewModels/Views)
     - Separare completă concerns: ViewModels fără UI logic, Services fără ViewModels
     - DI pattern cu extension methods pentru scalabilitate

9. **RESTful API Design Best Practices**
   - Sursa: Microsoft REST API Guidelines
   - Utilizare: Structură modele pentru API communication
   - Realizat în plus:
     - Modele separate per domeniu (Requests, Clients, Documents, Users, Device, etc.)
     - `PagedResponse<T>` generic pentru paginare
     - `QueryParameters` pentru filtrare/sortare consistentă
     - Service layer cu interfețe pentru testability

### 6.5 Proiecte și cod inspirație
10. **eShopOnContainers MAUI Client**
    - Sursa: https://github.com/dotnet-architecture/eShopOnContainers
    - Inspirație: Arhitectură HttpClient cu DI, error handling
    - Realizat în plus: Dual message handlers (auth + device tracking), logging avansat

11. **MAUI Weather App Sample**
    - Sursa: https://github.com/davidortinau/WeatherTwentyOne
    - Inspirație: C# Markup patterns, animații
    - Realizat în plus: Business logic complexă (workflow cerințe cadastrale), multiple domenii API

### 6.6 Ce s-a realizat în plus față de sursele de bază

1. **Arhitectură enterprise-grade complet customizată:**
   - Pattern DI cu extension methods în `DI.cs` (nu există în samples standard)
   - Chaining de HTTP message handlers (auth + device tracking)
   - Separare strictă domenii (9 servicii API separate cu interfețe)

2. **Zero XAML UI (cu excepția Shell și App.xaml):**
   - Toate Views în C# Markup pur
   - Pattern repeatable cu ViewModel injection în constructor
   - Performance superior față de XAML parsing

3. **Device tracking complet:**
   - Generare unică device ID persistentă
   - Auto-register/deregister la login/logout
   - Injectare automată device ID în toate request-urile HTTP
   - Sincronizare bidirectională cu backend

4. **Sistem notificări integrat:**
   - Firebase FCM cu Plugin.Firebase
   - `NotificationService` pentru routing notificări la ecranele corecte
   - Token refresh automat, error handling

5. **Multi-domain business logic:**
   - 9 domenii separate: Requests, Clients, Documents, Files, Users, Device, Accounts, Analytics, Migrations
   - Modele complexe cu relații (cerințe → clienți → documente → fișiere)
   - Query parameters pentru filtrare/sortare/paginare avansată

6. **Security features avansate:**
   - SecureStorage pentru tokens și device ID
   - Auto-refresh tokens cu fallback la login
   - Logout complet cu device deregistration
   - Custom URL scheme pentru OAuth redirects

7. **Developer experience optimizat:**
   - Logging comprehensive cu ILogger
   - Error handling centralizat
   - Cod organizat, comentat, scalabil
   - Readme.md complet cu reguli stricte de dezvoltare

### Proiecte anterioare care au influențat arhitectura:
- **Niciun proiect anterior direct replicat** - aceasta este o implementație custom de la zero
- Experiența generală din proiecte enterprise .NET și MAUI a ghidat deciziile arhitecturale
- Pattern-urile sunt adaptate specific pentru domeniul cadastral și cerințele SIGL

---

## Concluzie

SIGL Cadastru Mobile reprezintă o soluție modernă, enterprise-grade pentru digitalizarea și mobilizarea proceselor cadastrale. Prin utilizarea celor mai recente tehnologii .NET MAUI, arhitectură MVVM strictă, autentificare securizată și notificări în timp real, aplicația oferă o experiență superioară atât pentru utilizatorii finali cât și pentru dezvoltatori.

Aplicația se diferențiază de competiție prin:
- **Performanță**: UI nativă în C# Markup (nu hybrid web)
- **Securitate**: Keycloak/OIDC enterprise-grade
- **Funcționalitate**: Workflow complet, nu doar viewer
- **Extensibilitate**: Arhitectură modulară, ușor de extins

Potențialul de monetizare este semnificativ pe piața românească și est-europeană, unde digitalizarea serviciilor cadastrale este în plină expansiune.
