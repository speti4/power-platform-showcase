# Fejlesztési terv az xFlower szerződéskezelő alkalmazás Power Platform alapú kiváltására

## [Alapfogalmak, áttekintés](/power-platform-overview.md)

## Kiindulási helyzet és célkitűzések

Az xFlower egy szerződéskezelő rendszer, amely több összetett folyamatot támogat: szerződéskötés, módosítás, felmondás, véleményezés, javítás, stb. A cél a meglévő funkciók teljes körű újraimplementálása a [Microsoft Power Platform](/power-platform-overview.md/#mi-a-power-platform) eszközeivel (Microsoft Dataverse, Power Apps és Power Automate) úgy, hogy a folyamatok azonos módon működjenek, de a megoldás rugalmasan bővíthető legyen a jövőben. A tervezést a Microsoft hivatalos ajánlásai mentén végzem, figyelembe véve az alkalmazás architektúráját, az ALM-et (alkalmazás-életciklus menedzsment), az adatvédelmet és a licencelést.

A következőkben részletesen bemutatom a javasolt architektúrát, a komponensek felépítését, egy lehetséges fejlesztési ütemezést, "quick win" fejlesztéseket, valamint az üzemeltetési és megfelelőségi szempontokat.

## Javasolt architektúra és főbb komponensek

A megoldás egy Microsoft Dataverse alapú adatplatformra épül, egy Power Apps üzleti alkalmazásra (várhatóan modell-driven app), valamint számos Power Automate folyamatra az automatizációhoz. Az architektúra célja, hogy az xFlower minden fő funkcióját lefedje a Power Platform ökoszisztémában, miközben moduláris felépítésű marad a későbbi bővítésekhez.

- **[Dataverse](/power-platform-overview.md/#mi-az-a-dataverse) (adatbázis):** A szerződésadatok központi tárhelye lesz, ahol az összes szerződéssel kapcsolatos adat és metaadat strukturáltan tárolódik. A Dataverse üzleti adatok kezelésére tervezett, biztonságos adatbázis, amely támogatja a komplex adatmodelleket és üzleti logikát (például kapcsolatok, üzleti szabályok, beépített validáció). Emellett a Dataverse megfelel a vállalati biztonsági előírásoknak, mivel az adatok titkosítva vannak mind átvitel közben, mind nyugalmi állapotban (a Dataverse SQL-alapú adatbázisai átviteli titkosítást – TLS –, és nyugalmi titkosítást – Transparent Data Encryption – alkalmaznak)
[🔗Link](https://learn.microsoft.com/en-us/power-platform/admin/wp-compliance-data-privacy)
- **[Power Apps](/power-platform-overview.md/#mi-az-a-power-apps) (alkalmazásréteg):** Egy dedikált Power Apps alkalmazás biztosítja a felhasználói felületet a szerződések kezeléséhez. A tervek szerint egy modellvezérelt alkalmazást használunk, mivel ez gyorsan kialakítható a Dataverse adattáblákra építve, és natívan támogatja az űrlapokat, nézeteket, üzleti folyamatokat és jogosultságkezelést. A modellvezérelt appok ideálisak összetett, adatközpontú üzleti alkalmazásokhoz, ahol egységes felületre és beépített adatkapcsolatokra van szükség. Szükség esetén kiegészítő canvas oldalak vagy egyedi oldalak is kialakíthatók speciális igényekhez (például egy egyszerűsített űrlap mobileszközökre), de az elsődleges felület a modellvezérelt app lesz a hatékonyság miatt.
- **[Power Automate](/power-platform-overview.md/#mi-az-a-power-automate) (folyamat-automatizálás):** A szerződéskezelés folyamatlogikáját felhőalapú workflowk valósítják meg. Különböző típusú Power Automate flow-kat tervezünk:
  - **Automatikus feladatok:** Olyan folyamatok, melyek egy adott adat-esemény hatására indulnak (Dataverse trigger). Például új szerződés rögzítésekor jóváhagyási folyamat indítása, vagy ha egy szerződés státusza "Aláírt" értékre vált, értesítés küldése az érintetteknek.
  - **Kézi indítású folyamatok:** A felhasználó által gombnyomásra indítható flow-k a Power Apps felületéről. Ilyen lehet például egy "Szerződéstervezet küldése véleményezésre" gomb, amely elindít egy folyamatot az adott szerződés adataival, és e-mailben kiküldi a dokumentumot a partnernek vagy belső jogi osztálynak.
  - **Ütemezett folyamatok:** Időzített alapon futó ellenőrzések, pl napi szinten futó flow a közelgő lejáratú szerződések felderítésére és figyelmeztető e-mailek / értesítések küldésére. A "szerződés lejáratra figyelmeztetés" funkció így valósulhat meg az új rendszerben.
  - **Jóváhagyási folyamatok:** A Power Automate Approvals funkciójával implementáljuk a szerződések belső jóváhagyását és véleményezését. Az Approvals lehetővé teszi aláírási/véglegesítési kérelmek küldését és nyomon követését, integrált döntéshozatallal. Így például egy szerződéstervezet belső jóváhagyására a rendszer automatizált jóváhagyási kérést küldhet a kijelölt vezetőnek vagy jogásznak, akik e-mailben vagy akár Microsoft Teamsben értesítést kapnak, és ott tudják elfogadni vagy elutasítani a kérelmet.
  [🔗Link](https://learn.microsoft.com/en-us/power-automate/get-started-approvals)
- **Integrációk és dokumentumkezelés:** A szerződésfájlok (Word/PDF dokumentumok) kezelésére több lehetőség van. A legegyszerűbb, ha a dokumentumokat a Dataverse entitásokhoz csatolt fájlként tároljuk. Ugyanakkor nagy mennyiségű vagy méretű dokumentum esetén javasolt a SharePoint integráció kihasználása. A Dataverse natív módon integrálható a SharePoint Online dokumentumtárakkal, így a szerződés rekordokhoz tartozó fájlok egy SharePoint tárba kerülhetnek. Ennek előnye, hogy a SharePoint-ban tárolt dokumentumok a rekord kontextusában is elérhetők, de akár a Power Apps alkalmazáson kívül, közvetlenül SharePoint felületről is hozzáférhetők azok számára, akiknek van jogosultságuk. [🔗Link](https://learn.microsoft.com/en-us/power-platform/admin/set-up-sharepoint-integration)
Ez növeli az együttműködést (a felhasználók közvetlenül megoszthatják vagy közösen szerkeszthetik a dokumentumokat SharePointban) és tehermentesíti a Dataverse tárhelyét.

  Emellett a Power Platform lehetővé teszi Word sablonok használatát a Dataverse adatokból, így a jövőben integrálhatunk egy funkciót, ami a rögzített adatok alapján automatikusan kitölt egy Word szerződés sablont. Ezzel kiváltható lenne az xFlower Word plug-in hasonló képessége, modern, felhőalapú módon. [🔗Link](https://learn.microsoft.com/en-us/power-platform/admin/using-word-templates-dynamics-365)

Az architektúra összességében rétegezett és moduláris: a Dataverse biztosítja a megbízható adatkezelést és logikai réteget (üzleti szabályok, kapcsolatok), a Power Apps a modern felhasználói felületet, a Power Automate pedig az automatizált folyamatokat és integrációkat. Ez a megközelítés megfelel a Microsoft által ajánlott low-code architektúrának, amely gyors fejlesztést és könnyű karbantarthatóságot tesz lehetővé.

## Adatmodell és a Dataverse entitások tervezése

Az xFlower rendszer folyamatai alapján felállítom a Dataverse entitásstruktúrát. A legfontosabb tervezési elv, hogy a meglévő adatok és kapcsolatok hiánytalanul átkerüljenek, és logikailag is illeszkedjenek. A főbb tervezett entitások és kapcsolatok:

- **Szerződés entitás:** Ez lesz a központi tábla, amely egyedi rekordként tárol minden szerződést. Ide kerülnek a szerződés metaadatai, pl: szerződésszám, tárgy, típus (keretszerződés, egyedi szerződés), a partner neve/azonosítója, kezdő és záró dátumok, státusz (Tervezet, Véleményezés alatt, Aláírásra vár, Aláírt, Hatályos, Lejárt, Felmondott, stb.), az adott iktatókönyv vagy üzleti terület, felelős személy, létrehozás dátuma stb.

  A státuszkezelést érdemes a Dataverse beépített State és Status Reason mezőivel megoldani, így egy szerződés lehet Aktív vagy Inaktív állapotú. Például ha egy szerződést felmondanak vagy módosítanak (és így már nem aktuális az eredeti), akkor az eredeti rekord Inaktív állapotba kerül. Ugyanez vonatkozik arra az esetre is, ha egy szerződést tárgytalanítanak (stornóznak) hibás rögzítés miatt – ilyenkor is Inaktív lesz, jelezve hogy nem érvényes. Az Inaktívvá tett rekordok megőrzik az adataikat, de alapértelmezés szerint nem jelennek meg az aktív listákban, csak archivált elemként. Ezzel biztosítható, hogy a hisztorikus adatok megmaradjanak nyomonkövethetőség céljából.
- **Szerződésmódosítás:** Az xFlower logikája szerint egy szerződés módosítása esetén keletkezik egy új szerződés rekord, ami össze van kapcsolva az eredetivel, és az eredeti lesz inaktiválva a folyamat végén. Ezt a kapcsolatot a Dataverse-ben egy önreferenciáló kapcsolatként valósítjuk meg a Szerződés entitáson belül: például "Eredeti szerződés" (lookup mező saját magára). [🔗Link](https://learn.microsoft.com/en-us/power-apps/maker/data-platform/define-query-hierarchical-data)

  Ha egy szerződés módosítással jött létre, ez a mező hivatkozni fog az elődjére. Ennek köszönhetően láncolhatóak a szerződések (egy eredeti és több egymást követő módosítás kapcsolata), és lekérdezhető, hogy egy adott aktív szerződésnek mi volt az eredeti verziója. A módosítás folyamata végén a rendszer a Power Automate segítségével automatikusan beállítja ezt a kapcsolatot és inaktiválja az előd rekordot. Az eredeti xFlower működésből átvesszük azt is, hogy a korábbi aláírt dokumentumok átkerüljenek az új módosítás rekordjához – ezt az integrált dokumentumkezelésnél a SharePoint vagy Dataverse file csatolmányok átmásolásával érjük el.
- **Szerződésjavítás és tárgytalanítás:** Az xFlower megkülönbözteti a kisebb hibajavításokat, melyek nem eredményeznek új szerződésszámot. Ezt a Power Platformon nem külön entitással, hanem a Szerződés entitás egy tranzakciójával kezeljük. Például ha egy rögzített szerződés adatában hibát kell javítani, a jogosult felhasználó az alkalmazásban módosíthatja a rekord megfelelő mezőit egy "Javítás" művelettel (amit lehet, hogy csak bizonyos státuszban engedünk meg, pl ha még nincs aláírva, vagy külön jóváhagyással). A javítás tényét naplózhatjuk egy megjegyzésben vagy verziókövetéssel. Ha egy teljes szerződést kell tárgytalanítani, például duplikáció miatt, akkor a folyamat annyi, hogy a rekord státuszát a rendszer Inaktív - Tárgytalanított értékre állítja (vagy egy egyedi mezővel jelöljük tárgytalanítottnak). Mivel ilyenkor nem keletkezik új szerződés, a rekord megmarad a rendszerben, de azonosítja magáról, hogy érvénytelen. Ez összhangban van az xFlower logikájával, miszerint tárgytalanításnál nem jön létre új szerződésszám, nem változik a partnerrel kötött megállapodás, csak a rendszerbeli nyilvántartást igazítjuk ki.
- **Partnerek / Ügyfelek entitás:** A szerződő feleket célszerű külön entitásban nyilvántartani, hogy ne kelljen minden rekordnál ismételten rögzíteni az adatokat. A Dataverse-ben használhatjuk az alapértelmezett Account és Contact táblákat, ha azok megfelelnek, vagy létrehozhatunk egy egyedi Partner entitást. Ebben tároljuk a partner nevét, címét, kapcsolattartókat stb. A Szerződés entitás egy keresőmezővel kapcsolódik a partnerhez. Így később könnyen listázhatjuk egy partner összes szerződését, illetve elkerüljük az adatredundanciát. Mivel a partneradatok jelenleg is a CRM rendszerben vannak, integrációval is át lehet venni az adatokat vagy valós időben lekérdezni. A CRM lehetőségeit és korlátait figyelmbevéve használhatunk Dataflow-t vagy Custom Data Connectort.
- **Dokumentumok, csatolmányok:** A szerződésekhez tartozó fájlokat (tervezetek, aláírt szerződések, egyéb levelezések) kezelnünk kell. Dataversen bármelyik entitásnál engedélyezhetjük a mellékletek csatolását, de ennél fejlettebb megoldás a Serverbased SharePoint dokumentumkezelés használata. [🔗Link](https://www.youtube.com/watch?v=8zCcIQ_gpB8&t=420s)

  Minden szerződéshez a rendszer automatikusan létrehoz egy dedikált SharePoint mappát dokumentumtár-integrációval, ahová a felhasználók feltölthetik a vonatkozó dokumentumokat. A Power Apps felületen ezek a dokumentumok listázhatók és megnyithatók lesznek, a model-driven app natívan támogatja a dokumentumok megjelenítését. Így biztosítjuk, hogy például az xFlower-ben említett "egyéb levelezés csatolása a szerződéshez" funkció is megmarad: a felhasználó feltöltheti az email (.msg) fájlokat vagy PDF-eket a szerződés SharePoint mappájába, és bárki, aki hozzáfér a szerződéshez, meg tudja tekinteni azokat. A SharePoint jogosultságokat összhangban tartjuk a Dataverse jogosultságokkal, a jogosultságokat groupokkal fogjuk szabályozni.
- **Egyéb entitások és metaadatok:** Szükség lehet további támogató entitásokra is, például:
  - *Szerződés típusok / kategóriák:* listázza az előre definiált szerződés típusokat (pl beszállítói szerződés, bérleti szerződés, stb), amelyeket legördülő listából lehet választani a szerződés rögzítésekor.
  - *Iktatókönyv:* ha a szerződéseket iktatókönyvenként vagy üzleti területenként kell nyilvántartani, ezt kétféleképpen kezelhetjük. Vagy egy mezővel jelöljük a szerződésen az iktatókönyvet, vagy kihasználjuk a Dataverse Business Unit koncepcióját, és üzleti egységenként különíthetjük el a rekordokat. Ezt a [biztonság résznél](#jogosultságkezelés-és-biztonság) kifejtem.
  - *Jóváhagyási kérések és feladatok:* Habár a jóváhagyási folyamatokat főként Power Automate-tel intézzük, létrehozhatunk egy entitást a jóváhagyási feladatoknak is, hogy a rendszerben nyilvántartsuk, kinél áll épp a szerződés és milyen döntés született. A Power Automate flow írhatja ezen entitás rekordjait. Pl: új bejegyzés "Szerződés X véleményezése - folyamatban", majd frissítés "jóváhagyva" vagy "elutasítva". Ez nem kötelező, mert a Power Automate Approvals saját nyilvántartással is rendelkezik, de egy testreszabott nyilvántartás előnyös lehet riportolási célokra.
  - *Napló és audit:* A Dataverse beépített audit naplózása lehetővé teszi, hogy minden adatváltozást megőrizzünk. Ezt engedélyezni fogjuk a kritikus entitásokra, hogy teljesüljön a nyomonkövethetőség követelménye. Például: "kinél áll a szerződés, mióta" ezt kombináltan a jóváhagyási entitás és az audit adatok biztosítják.

Az adatmodell tervezésénél ügyelni kell a normalizáltságra és a jövőbeni bővíthetőségre. Ha új folyamat kerül bevezetésre (pl. szerződés hosszabbítás, megújítás külön folyamattal), a meglévő struktúrába illeszkedve lehessen megvalósítani, minimális átalakítással. **Szánjunk időt egy robusztus adatmodell megtervezésére, és implementáljunk RBAC-t már a tervezési fázisban.**

## Alkalmazás (Power Apps) tervezése és funkciók

A felhasználók számára készült alkalmazás várhatóan egy model-driven Power Apps lesz, mely a Dataverse entitásokra épül. Ennek oka, hogy a modellvezérelt app:

- Gyorsan konfigurálható a Dataverse adatokra -> kevesebb egyedi fejlesztést igényel, sok funkció kattintgatással beállítható
- Egységes és felhasználóbarát Dynamics 365-szerű felületet biztosít, amely támogatja a listákat, űrlapokat, nézeteket, diagramokat és a folyamatvezérlést [🔗Link](https://learn.microsoft.com/en-us/power-apps/maker/model-driven-apps/model-driven-app-overview)
- Beépítetten támogatja az üzleti folyamatok (Business Process Flow) vizuális megjelenítését a űrlap tetején, ami kiválóan alkalmas a szerződéskötési folyamat fázisainak követésére [🔗Link](https://learn.microsoft.com/en-us/power-apps/user/work-with-business-processes)

### Fő felületek és funkciók

- **Navigáció és modulok:** A model-driven appban létrehozunk egy jól strukturált oldalsó menüt az alapvető modulokkal: pl. Szerződések, Partnerek, Jóváhagyások, Keresés/Lista és esetleg Riportok/Dashboardok. A nyitóoldalon egy dashboard jelenhet meg, amely mutatja a felhasználóhoz tartozó nyitott feladatokat (pl. "Önre váró véleményezések"), a közelgő lejáratú szerződéseket számlálóval, stb.
- **Szerződés űrlapok:** A Szerződés entitáshoz több űrlapot is kialakítunk:
  - Új szerződés létrehozása űrlap – itt a felhasználó megadja a szükséges adatokat egy tervezet rögzítéséhez. Ide integrálhatunk üzleti logikát: pl. bizonyos mezők kitöltése kötelező, egyes mezők értékei függnek a szerződés típusától. Ezeket Business Rule-okkal vagy komplexebb esetben JavaScript/Power Fx kóddal is lehet kezelni.
  - Megtekintő űrlap / view – a már rögzített szerződés adatai és kapcsolódó elemek (kapcsolódó módosítások, csatolmányok, jóváhagyások) megtekinthetők. Itt lesznek gombok a folyamatlépések indítására is (pl. "Véleményezés indítása", "Módosítás indítása", "Felmondás").
  - Szerkesztő űrlap – bizonyos állapotokban engedélyezzük a szerződés adatainak módosítását, pl. Javítás esetén vagy Tervezet fázisban. Ha a szerződés Aláírt/Active státuszban van, a mezők tipikusan csak olvashatók. Ezt jogosultságokkal és űrlapszabályokkal beállítjuk.
  - Az űrlap tetejére az adott szerződéshez tartozó Business Process Flow folyamatábra kerül, amely végigvezeti a felhasználót a szerződéskötés lépésein. Például egy Szerződéskötés BPF fázisai lehetnek: Tervezet létrehozása -> Belső véleményezés -> Külső véleményezés (opcionális) -> Aláírás -> Lezárás. A BPF vizuálisan jelezheti, hogy épp kinél van a labda és mióta. Ezzel megvalósul az a követelmény, hogy "követhetők legyenek a szerződéskötési fázisok". Hasonló BPF-ek készíthetők a módosítás és felmondás folyamatokra is, vagy egyetlen BPF több ággal.
- **Lista nézetek és keresés:** Különböző nézeteket (views) hozunk létre a Szerződés entitásra:
  - Aktív szerződések: minden hatályos szerződés
  - Függőben lévő tervezetek: még alá nem írt, folyamatban levő ügyek
  - Hamarosan lejáró szerződések: pl következő 30 napban lejárók
  - Inaktív (lezárt, felmondott, módosított) szerződések archiválva. Az app natívan támogatja az Advanced Find jellegű kereséseket, illetve a global search funkcióval a felhasználók gyorsan megtalálhatnak bármilyen szerződést név, szám, partner alapján. Ez legalább olyan hatékony lesz, mint az xFlower beépített keresője.
- **Gombok és integrált műveletek:** Az űrlapokon és nézeteken egyedi parancsgombokat konfigurálunk a gyakori műveletekhez. Például:
  - *"Szerződés véleményezése":* gomb megnyomásával a háttérben meghívunk egy Power Automate folyamatot, ami összegyűjti a szükséges adatokat és elküldi az érintett felelősöknek emailben a Power Automate Approvals keretében jóváhagyásra. A felhasználó a gomb megnyomásakor egy panelen kiválaszthatja, hogy belső vagy külső véleményezés indul, kit kell bevonni stb.
  
    Ez hasonlóan működik, mint az xFlower-ben említett "belső véleményeztetésre küldés" és "külső partner véleményezésre küldés közvetlenül a rendszerből", azzal a különbséggel, hogy a Power Platform megoldással mindezt a felhasználóbarát app felületen belül tesszük meg, a háttérben a flow automatikusan e-mailt küld a dokumentummal, anélkül, hogy a felhasználónak külön le kellene töltenie azt és levelezőben csatolnia.
  - *"Módosítás indítása":* ez a gomb egy olyan folyamatot indít, ami létrehoz egy új szerződés rekordot előre kitöltve az aktuális szerződés adataival (Dataverse plugin vagy Power Automate segítségével), beállítja az előző mezőt (link az eredetihez), átveszi az aláírt dokumentumokat, és az új rekordot nyitja meg a felhasználónak szerkesztésre.
  
    Ezzel nagyon hasonló módon működik, mint az xFlower szerződés módosítás folyamata. A különbség az, hogy a Power Apps-ben ez felhasználó által kezdeményezett műveletként jelenik meg, nem külön modul indítja, hanem a meglévő szerződés nézetéből indul.
  - *"Felmondás indítása":* hasonlóan, egy folyamat, ami lekérdezi a felhasználótól a szükséges információkat (felmondás indok, dátum stb.), majd vagy létrehoz egy Felmondás rekordot, ha külön nyilvántartjuk, vagy egyszerűen frissíti a szerződés státuszát Felmondás alatt értékre és figyeli, míg a felmondó dokumentum feltöltésre kerül. A folyamat végén beállítja a szerződést Inaktívra. Ez megfelel az xFlower felmondási folyamatának logikájának​.
  - *"Dokumentum generálás":* jövőbeli fejlesztésként egy gombbal a felhasználó Word sablon alapján legenerálhatja a szerződés szövegét a rögzített adatokból. Erre használhatjuk a Dataverse Word template funkcióját [🔗Link](https://learn.microsoft.com/en-us/power-platform/admin/using-word-templates-dynamics-365), vagy alkalmazhatunk Power Automate flow-t is.
- **Értesítések:** A felhasználói élmény javítása érdekében bizonyos eseményeknél felugró értesítéseket (native értesítés mobilra) vagy email/Teams üzeneteket állíthatunk be. Például amikor egy partner visszaküldte aláírva a szerződést és azt feltöltik, a rendszer automatikusan kiküld egy "Szerződés aláírva" értesítést az érintetteknek.

A model-driven alkalmazás testreszabása során betartjuk a Microsoft best practice-eket: minden testreszabást Solution-be csomagolva végzünk, nem a default environment "alapértelmezett megoldásában". A Microsoft határozottan javasolja saját Solution használatát a fejlesztői munkához, hogy a komponensek könnyen átvihetők legyenek másik környezetbe, és konzisztensen alkalmazzuk a prefixeket. Ennek köszönhetően az alkalmazásunk könnyen telepíthető lesz teszt és éles környezetbe is, és egyértelműen nyomon követhetőek lesznek a fejlesztési lépések, későbbi módosítások. [🔗Link](https://learn.microsoft.com/en-us/power-platform/alm/implement-healthy-alm)

## Üzleti folyamatok megvalósítása a Power Platformon

Fontos kiemelni, hogyan fognak az egyes xFlower üzleti folyamatok működni a Power Platformon. A cél az, hogy a folyamatok lépései és szabályai változatlanok maradjanak, csak a technológiai háttér változik. Az alábbiakban összefoglalom a főbb folyamatokat és azok új megvalósítását:

- **Szerződéskötés folyamata:** Új szerződés létrehozásakor a felhasználó kitölti a szerződés űrlapot és elindítja a jóváhagyási/véleményezési kört. A Power Automate jóváhagyási folyamat kezeli a belső véleményezést: kiküldi a tervezetet a jogi osztálynak vagy vezetőknek, akik a Power Automate Approvals keretében hagyják jóvá vagy javasolnak módosítást. A rendszer rögzíti a döntéseket, és ha minden belső jóváhagyó rábólintott, akkor a folyamat továbblép.
  
  Következő lépés a külső véleményezés/egyeztetés: a rendszer emailben kiküldi a szerződéstervezetet a partnernek (vagy megoszt egy fájlra mutató Sharepoint linket). A partner által visszaküldött és egyeztetett dokumentumot a felhasználó feltölti a szerződéshez. Ezt követi az aláírás fázis: a rendszer jelezheti, hogy nyomtatás és fizikai aláírás szükséges (ha nincs integrált e-aláírás megoldás), majd amikor az aláírt példányt feltöltik, a felhasználó megjelöli a szerződést Aláírt-nak. Ekkor a folyamat lezárul: a BPF utolsó szakaszába ér, a státusz Aktív/Hatásos, és értesítést kap minden érintett, hogy a szerződés életbe lépett.
- **Szerződés módosítás folyamata:** A felhasználó egy meglévő aktív szerződésnél indítja a Módosítás actiont az alkalmazásban. Ennek hatására egy Power Automate folyamat létrehoz egy új szerződés rekordot, átörökítve az előzmény releváns adatait és dokumentumait, valamint beállítja a Kapcsolódó eredeti szerződés mezőt. Innentől a módosítás gyakorlatilag ugyanolyan lépéseken megy át, mint egy új szerződés (belső és/vagy külső véleményezés, aláírás). A BPF esetében lehet külön Módosítás BPF-et használni, de akár a szerződéskötési BPF újrafutása is elképzelhető az új rekordra. Végül amikor a módosított szerződés aláírt státuszba kerül, a flow automatikusan Inaktívra állítja az eredeti szerződést, ahogy azt az xFlower is teszi.
- **Szerződés felmondás folyamata:** A felmondást az adott aktív szerződés nézetéből indítja a felhasználó (Felmondás gomb). A háttérfolyamat bekéri a szükséges adatokat (felmondás dátuma, indoka, csatolandó felmondó levél stb.). Ekkor is opcionálisan lehet jóváhagyást kérni (pl. a felmondást jóváhagyatni kell vezetővel, mielőtt elküldik a partnernek – ezt a Power Automate egy belső approval formájában megoldhatja). A partner felé kimenő felmondó levelet a rendszer sablon alapján előállíthatja, vagy a felhasználó feltölti a partner által aláírt felmondást vissza. A folyamat végén a szerződés rekordját Inaktív státuszúra állítjuk és jelöljük, hogy felmondásra került. Mivel új szerződés nem keletkezik, a kapcsolat mezőt nem használjuk, csak a státuszt frissítjük. Ha mégis külön nyilvántartanánk a felmondási eseményt, létrehozhatunk egy Felmondás entitást, amely kapcsolódik a szerződéshez, de valószínűleg elég a státusz és az audit.
- **Szerződés véleményezési folyamat:** Ez nem külön folyamatként indul, hanem az új rendszerben a szerződéskötés/módosítás részeként Power Automate jóváhagyások által valósul meg. Ha szükség van önálló véleményezési folyamatra (pl. a szerződés előkészítése alatt többszöri iteráció a jogásszal még a formális indítás előtt), azt is támogatjuk: lehet egy Véleményezés kérése gomb, ami bármikor használható a tervezet stádiumban, és belső körben körözteti a dokumentumot. Az így csatolt levelezés, javaslatok mind a rendszerben maradnak (pl. a SharePoint mappában tárolva vagy a Jegyzetekhez csatolva), így az utólagos visszakereshetőség biztosított.
- **Szerződés javítás/tárgytalanítás folyamata:** Ahogy az adatmodellnél írtam, ez nem hoz létre új szerződést. A felhasználó a hibás adatot javíthatja, illetve ha teljes szerződést kell tárgytalanítani, azt egy Tárgytalanítás művelettel teheti meg. Ekkor a rendszer megkérdezi, hogy valóban tárgytalanítja-e a szerződést, figyelmeztetve, hogy ezzel archív státuszba kerül. Jóváhagyáshoz köthetjük ezt is (pl. csak admin engedélyezheti egy szerződés törlését/tárgytalanítását, nehogy véletlenül történjen). Ha jóváhagyják, a rekord inaktiválódik és megjegyzést fűzünk hozzá (pl. "Tárgytalanítva XY okból, dátum, felhasználó"). Mivel nem keletkezik új szerződés, a nyilvántartás sorszáma nem változik.

Összességében a Power Platform lehetővé teszi, hogy minden jelenlegi folyamatot leképezzünk, gyakran még hatékonyabb módon. A felhasználók egy egységes appban láthatják és kezelhetik a szerződések teljes életciklusát, a folyamatok pedig automatizáltan végbemennek a háttérben. Az architektúra nyílt arra, hogy új folyamatokat is hozzáadjunk (pl. hosszabbítás, megújítás, rendszeres felülvizsgálat), ezek a Dataverse meglévő adataira és a hasonló Power Automate sablonokra támaszkodva gyorsan implementálhatók.

## Jogosultságkezelés és biztonság

A szerződéskezelő alkalmazás jellegéből fakadóan kiemelt figyelmet kap a jogosultságkezelés. Biztosítani kell, hogy minden felhasználó csak azokat az adatokat és funkciókat érje el, amelyekre jogosult, összhangban a szerepkörével. Például egy üzletkötő lássa a saját szerződéseit, a jogász láthassa az összeset a saját területén, de csak véleményezési jogosultsággal, stb. A Microsoft Dataverse erre egy szerepkör-alapú biztonsági modellt (RBAC) kínál. [🔗Link](https://learn.microsoft.com/en-us/power-platform/admin/wp-security-cds)

- **Biztonsági szerepkörök:** Meghatározzuk az üzleti szerepeket és ezekhez hozzárendelünk Dataverse biztonsági szerepköröket. Például:
  - *Szerződés kezdeményező* – jogosult új szerződéseket rögzíteni, a saját maga által létrehozott szerződéseket látja (vagy a saját üzleti egységén belülieket, igénytől függően), nem feltétlen lát más osztályok szerződéseit
  - *Jogi véleményező* – hozzáfér a rá bízott szerződésekhez (pl. egy jogi business unit tagjaként látja az adott unit összes szerződését). Módosíthat bizonyos mezőket, pl jogi megjegyzések, és használhatja a Véleményezés jóváhagyása/elutasítása funkciót, de nem feltétlen kezdeményezhet új szerződést
  - *Manager* – lehet jogosult minden szerződést olvasni a saját szervezeti egységében, és jóváhagyási döntéseket hozni
  - *Power user* (én🫡) – teljes hozzáféréssel bír minden adathoz és beállításhoz, ő kezeli a törzsadatokat (pl. partnerlista, kategóriák) és konfigurálja az alkalmazást
  - *Olvasó* – opcionálisan lehet olyan szerepkör, aki csak olvasásra jogosult bizonyos körben (pl. audit vagy megfigyelő szerep)

A szerepköröket az elvárásokhoz igazítjuk, és a Dataverse szerepkörökön belül finoman szabályozzuk: mely entitásokat érheti el (Szerződés, Partner, stb.), azon belül milyen műveleteket végezhet (Create, Read, Write, Append, AppendTo, stb.), és milyen rekordszinten (saját, üzleti egység, szervezet szintjén). A Microsoft best practicest követve a legkisebb jogosultság elvét alkalmazzuk, minden szerepkörnek csak a munkájához minimálisan szükséges jogosultságokat adjuk meg. Így csökkentjük az adatvédelem kockázatait és elkerüljük, hogy bárki hozzáférjen érzékeny adatokhoz indokolatlanul.

- **Üzleti egységek (Business Units):** A Dataverse lehetővé teszi az adatokat üzleti egységek szerint szeparálni. [🔗Link](https://learn.microsoft.com/en-us/power-platform/admin/wp-security-cds#business-units) Az xFlower dokumentumban említett iktatókönyvek leképezésére létrehozhatunk több business unitet a környezetben, és a felhasználókat hozzárendeljük a megfelelő BU-hoz. A szerződés rekordok pedig létrehozáskor a felhasználó üzleti egységéhez fognak tartozni. Így beállítható olyan szerepkör, amely Business Unit szintű olvasási/írási jogot ad: az adott egység tagjai látják egymás szerződéseit, de más BU-k adatait nem. Ha egyes felhasználóknak több BU-hoz kell férniük, használhatunk team-eket a megosztásra, vagy a felhasználó áthelyezhető a megfelelő BU-ba az adott rekord kapcsán. A Dataverse biztonsági modell hierarchikus is lehet – ha egy felsővezetőnek mindent látnia kell, lehet egy parent-child BU hierarchia, ahol a felső BU-ban lévő szerepkör szervezet szintű olvasási jogot is kap.
- **Mezőszintű és soralapú biztonság:** A Dataverse tud mező / oszlopszintű biztonságot is, de várhatóan erre nem lesz külön szükség, mert a folyamatainkat szerepkörökkel jól le tudjuk fedni. Ha mégis van pl bizalmas pénzügyi adat, amit csak bizonyos szerepkör láthat, azt védhetjük oszlopszintű biztonsági profilokkal. A rekord / soralapú megosztást az üzleti egységek és csapatok kombinációjával oldjuk meg, nem egyesével osztogatunk rekordokat, mert az nehezen menedzselhető. Az is kulcsfontosságú, hogy egyszerre több szerepkör is lehet egy usernek, és ezek halmozódnak. Dataverseben ha valakinek több szerepköre van, a legmagasabb privilégium szint érvényesül az összes közül.
- **Hitelesítés és hozzáférés:** A Power Apps a céges Azure AD hitelesítést használja, ami támogatja a többtényezős hitelesítést. Külső felhasználók közvetlenül nem fognak hozzáférni a belső apphoz. (Ha mégis szeretnénk ezt, arra Power Pages portált lehetne létrehozni a jövőben). A felhasználók jogosultságait a Power Platform Admin Centerben rendeljük hozzá. Célszerű Azure AD csoportokat használnunk a szerepkörök kiosztásához, egyszerűsíti a kezelést.
- **Adatvédelem és audit:** A biztonság része az is, hogy nyomon tudjuk követni, ki fért hozzá az adatokhoz és ki milyen változtatást végzett. Bekapcsoljuk a Dataverse Audit napló funkcióját a szerződés adatokra. Az audit adatok default 30 napig a rendszerben maradnak, és utána is kiexportálhatók archiválás céljából​. Szükség esetén hosszabb ideig is megőrizhetők az audit adatok Purview vagy más archív megoldással. [🔗Link](https://learn.microsoft.com/en-us/power-platform/admin/wp-compliance-data-privacy)

Összefoglalva, a tervezett biztonsági modell biztosítja, hogy minden felhasználó csak a munkájához szükséges adatokhoz férjen hozzá, egyúttal rugalmasan lehet csoportokat és új szerepköröket kezelni.

## Quick Wins - fejlesztési ötletek

Az alábbi ötletek közös jellemzője, hogy kis ráfordítással megvalósíthatók, nem gátolják a fő fejlesztési ütemtervet, viszont jelentős üzleti értéket demonstrálnak. Ezeket párhuzamosan, a prototípus fázisban is elkezdhetjük:

1. **Szerződés-lejárat értesítések:** Egy ütemezett Power Automate folyamat, ami például minden reggel ellenőrzi a Dataverseben, van-e 30 napon belül lejáró aktív szerződés, és ha igen, küld egy összefoglaló e-mailt vagy egy Teams üzenetet az érintett felelős(ök)nek.
2. **Dynamics 365-szerű irányítópult:** Egy dashboard Power Appsben, ami grafikonon mutatja például a szerződések státusz szerinti megoszlását (hány tervezet, mennyi véleményezés alatt, hány aláírásra vár, stb), és listákat a felhasználóhoz rendelt aktuális feladatokról. Például egy kördiagram a státuszokról vagy a szerződés típusokról, illetve egy lista a "Közeli határidős feladatokról". [Minta1](/docs/model-driven-app-sample1.jpg), [Minta2](/docs/dynamics-365-ui.jpg)
3. **E-mail integráció és sablonok:** Például egy "Értesítés küldése partnernek" gomb, ami automatikusan generál egy e-mailt a partner kontaktjának a szerződés adataival. Ehhez kihasználhatjuk a Dataverse e-mail sablon funkcionalitását vagy a Power Automate e-mail küldést. Nem kell külön levelezőbe átlépni, csökkenti a hibázás esélyét és egységes formátumot ad a leveleknek. [🔗Link](https://learn.microsoft.com/en-us/power-apps/user/email-template-create)
4. **Mobil hozzáférés:** Mivel a Power Apps alapból responsive, könnyű megoldani, hogy a felhasználók mobilon vagy tableten is elérjék a szerződésadatokat Power Apps mobilappon keresztül. Készíthetünk egy rövid bemutatót arról, hogyan lehet egy szerződést lekérdezni vagy jóváhagyni telefonon.
5. **Microsoft Teams integráció:** A Power Automate Approvals jóváhagyási folyamat a Teams beépített funkciójára támaszkodik. A felelős jóváhagyó(k) email mellett Adaptive Card formában is kapnak értesítést, amiből egyből jóvá is hagyhatják vagy elutasíthatja a kérelmet, indokolhatják döntésüket. Ezen az elven működik a [Számla leadó app](/README.md/#számlaleadás-app) is.
6. **AI Builder előnézet:** Ha van rá lehetőség (AI builder credit), demózhatom, hogy a rendszer képes AI-val kiszedni információkat dokumentumokból, pl egy PDF-ből automatikusan kinyer bizonyos metaadatokat. Ez ugyan nem elsődleges, de egy prototípus szintjén meg tudom mutatni, hogy a platform milyen AI alapú bővíthetőséget kínál a jövőben.
7. **Power BI riport:** Adat alapú riportokat készíthetünk Power BI-ban is, amiket beágyazhatunk a Power Apps felületbe. Ehhez külön Power BI licencekre is szükség lenne az app felhasználóinak.

## Fejlesztési ütemterv és mérföldkövek

A projektet három fő szakaszra bonthatjuk:

|   | Leírás | Becsült időtartam |
|---|---------------------------------------------------------------------------------------------------|---|
| 1 | Interjú kulcsfelhasználókkal, PO-val -> megvalósítási terv véglegesítése, MVP funkciók kijelölése | 2 hét |
| 2 | Prototípus / MVP elkészítése                                                                      | 3-4 hét |
| 3 | Teljes funkcionalitású alkalmazás élesítése                                                       | 5-7 hét |

Az alábbi ütemtervet javaslom:

### Előkészületek (0. hét)

Ha a fejlesztőnek szánt [licencek](#licencelési-terv) rendelkezésre állnak, elkezdhető a [Dataverse environmentek](#környezetstratégia) létrehozása és hozzáférések beállítása. Ez előfeltétele a munka megkezdésének, mivel a licencekkel kapacitást is vásárolunk. Developer környezetben kezdjük a munkát, mely korlátozás nélküli Dataverse használatot enged.

### 1-2. hét - Interjúk, megvalósítási terv véglegesítés

- **Interjú kulcsfelhasználókkal:** felhasználói igények mentén módosítjuk, véglegesítjük a megvalósítási tervet
- **Minimal viable product (MVP):** meghatározzuk az MVP funkcióit
- **CRM kapcsolat meghatározása:** felmérjük, hogy custom data connectort és real time elérést, vagy időzített pl. napi adatáttöltés Dataflow-val lesz optimális
- **Sample data:** lehetőség szerint XFlower-ből exportált adatok és szerződésminták

### 3-6. hét – Alapok és prototípus

- **Elkészítjük a Dataverse adatmodellt:** létrehozzuk az entitásokat (Szerződés, Partner stb.), mezőket és kapcsolatokkal
- **Sharepoint site léterhozás:** létrehozunk 1-1 Sharepoint sitet teszt és éles Dokumentumtárnak. Az eléréseket mindenhol környezeti változókban kezeljük, így oldjuk meg, hogy életciklus menedzsment során ne essenek szét az entitások
- **Model-driven app első verziója:** legalább a Szerződés és Partner entitások űrlapjaival és nézeteivel
- **Implementáljuk az MVP funkciókat:** az interjúk során meghatározott funkciók felépítése
- **Funkcionális tesztelés:** Tesztadatokkal feltöltjük a rendszert, esetleg migrálunk néhány valódi szerződést az xFlower-ből teszt céljából és elkezdjük a funkcionális tesztelést
- Közben megvalósítunk **1-2 quick win funkciót** is a prototípus részeként (pl. lejárati értesítés, dashboard), hogy a demo még meggyőzőbb legyen
- A prototípus végére (5-6. hét) tartunk **egy bemutatót** a kulcsfelhasználóknak, begyűjtjük a **visszajelzéseket**.

### 7-11. hét – Teljes funkcionalitás implementálása

- **Összes folyamat lefedése:** megcsináljuk a Módosítás, Felmondás, Javítás folyamatok automatizációját Power Automate-tel, integrálva az alkalmazásba -> gombok és logika összekapcsolása
- **Finomítjuk az alkalmazás UI-t:** hozzáadjuk a szükséges űrlapmezőket, üzleti szabályokat (pl. dinamikus megjelenés/elrejtés), véglegesítjük a nézeteket
- **Szerepkörök és biztonság:** Elkészítjük a szerepköröket és beállítjuk a biztonságot. Ezt egy tesztfelhasználó készlettel validáljuk (pl. egy user, akinek csak olvasási joga van, valóban nem tud szerkeszteni stb.).
- **Jogosultsági logika beépítése:** Például egyes gombok csak akkor látszódjanak, ha a usernek van joga, vagy ha releváns a státusz
- **Integrációk:**  levelezés, SharePoint és CRM integráció beállítása. A SharePoint integrációt konfiguráljuk (dokumentumtár összerendelése a Dataverse entitással), és leteszteljük a fájl feltöltéseket.
- **User dokumentáció:** elkezdjük összeállítani a how-to doksikat. Ennek jó alapja lehet az XFlower jelenlegi dokumentációja, és a végleges fejlesztési terv

### 12-13. hét – Tesztelés, finomhangolás és élesítés

- Áttelepítjük az alkalmazást egy **UAT (User Acceptance Testing)** környezetbe vagy a végleges éles környezet egy előkészített verziójába. Ez a telepítés már Solution pack formájában történik, amit exportálunk a fejlesztői környezetből és importálunk a UAT környezetbe. Ekkor már a managed solution-t használjuk, hogy a végleges környezetben védjük a megoldást a véletlen módosításoktól. [🔗Link](https://learn.microsoft.com/en-us/power-platform/alm/solution-concepts-alm)
- **UAT:** A felhasználók, tesztelők a UAT-ban végigmennek a valós scenariokon, ellenőrzik, hogy minden folyamat az elvárt eredményt adja. Bíztatjuk a tesztelőket, hogy ne csak happy path teszteket csináljanak, és dokumentálják hiba esetén: a hiba keletkezésének helyét, várt eredményt és kapott eredményt. Itt még fel fog merülni pár módosítási igény vagy hibajavítás, amit a visszavezetünk a Dev környezetben, majd frissítésként adjuk át a Test / Prod környezetbe.
- **Elkészítjük a végleges migrációs tervet:** ha sok adatot kell áthozni az xFlower-ből, akkor az utolsó héten futtatunk egy migrációs szkriptet vagy Dataflow-t a régi rendszerből az újba. Ha kevés adat van, manuális importálást is lehet csinálni Dataverse import funkcióval. [🔗Link](https://learn.microsoft.com/en-us/power-apps/developer/data-platform/dataverse-odata-dataflows-migration)
- **Teljesítmény és terhelés teszt:** ez nem fog gondot okozni, de ha pl. több ezer szerződés van, megnézzük a lekérdezések sebességét, indexeket állítunk be ahol kell. Dataverse automatikusan is kezel indexeket a keresőmezőkhöz.
- **Élesítés:** A projekt végén az alkalmazást átvisszük éles környezetbe. Beállítjuk az éles adatforrásokat: SharePoint site, CRM, e-mail küldésnél végleges címek. A felhasználókat hozzárendeljük a megfelelő biztonsági szerepkörökhöz. Ekkor már a felhasználóknak rendelkezniük kell a szükséges [licencekkel](#licencelési-terv).
- **Az első éles futás során** a fejlesztő és kulcsfelhasználók szorosan figyelik a rendszert. Mivel a Power Platformon gyorsan lehet módosítani (egy konfiguráció változtatás és új megoldás import néhány órán belül kivitelezhető), az utolsó simításokra van lehetőség akár az élesítés utáni időszakban is, minimális kieséssel.

Ez a menetrend azt feltételezi, hogy dedikáltan tudok dolgozni a projekten. Az agilis iterációk elve mentén haladnék. Ha kéthetes sprint időszakokkal számolunk:

- Az interjú időszak és előkészületek: 1 sprint
- Prototípus / MVP: ~2 sprint
- Majd további 3-4 sprint időszak alatt építeném ki a teljes funkcionalitást, folyamatosan bevonva a business felhasználókat a visszajelzésekbe

Így 3-4 hónap alatt el lehet jutni a stabil éles verzióhoz.

## Licencelési terv

A fejlesztést egy Power Apps Developer környezetben végezzük, ami a fejlesztőnek teljes funkcionalitást biztosít (Dataverse, premium connectorok, korlátlan app és flow fejlesztés) díjmentesen a fejlesztéshez. Ezzel a prototípus és a fejlesztés során nem merülnek fel licenckorlát miatti akadályok. Üzleti adatok szempontjából ez egy különálló, teszt jellegű környezet marad. [🔗Link](https://learn.microsoft.com/en-us/power-platform/developer/create-developer-environment)

### Usereknek

Az éles rendszer felhasználói számára a javaslat a **Power Apps per app licencek** használata. Ez azt jelenti, hogy minden felhasználó, aki használni fogja az új szerződéskezelő appot, rendelkezik egy alkalmazásonkénti licenccel, amely feljogosítja egy meghatározott Power App (illetve a hozzá tartozó folyamatok) futtatására. A per app licenc magában foglalja a Dataverse (premium adatforrás) használatát és a kapcsolódó Power Automate flow-k futtatását az adott alkalmazás kontextusában. A Microsoft dokumentációja alapján a per app licencet nem közvetlenül a felhasználókhoz rendeljük hozzá, hanem először a Power Platform admin centerben allokáljuk az adott környezethez, majd amikor az appot megosztjuk a felhasználóval, automatikusan fogyaszt egy licencet a keretből. Ez rugalmas, mert pl. 20 felhasználóra veszünk 20 per app licencet, az admin centerben hozzárendeljük az éles környezethez, és utána ők gond nélkül használhatják az appot. [🔗Link](https://learn.microsoft.com/en-us/power-platform/admin/about-powerapps-perapp)

[Listaára: €4.70 / user / hó](https://admin.microsoft.com/#/catalog/offer-details/power-apps-per-app-plan-1-app-or-website-/E222746C-D5A0-4F63-B742-CC355EC3BBB6)

Fontos kitétel: a per app licenc alkalmazásonként értendő. Bár ez nem fog előfordulni ,de amennyiben mégis külön appokra bontanánk, ügyelni kell rá, hogy egy felhasználó per app licence alapértelmezetten 1 app használatát fedezi (2021 októbere óta egy licenc = 1 app vagy 1 portal). Tehát ha két appot is használna ugyanaz a user, két licencet kell kiosztani neki.

### Fejlesztő / rendszergazda

A fejlesztő (én🫡) számára Power Apps Premium és Power Automate Premium (⚠️NEM per user) licencek kellenek. A Power Apps licenc lehetővé teszi, hogy korlátlan számú alkalmazást készítsen az adott tenantban, nem csak egyet egy adott környezetben. Ez szükséges a fejlesztéshez, adminisztrációhoz és életciklus menedzsmenthez. A Power Automate Premium licenc pedig engedi, hogy olyan flow-kat is készítsünk és futtassunk, amik nem kapcsolódnak közvetlenül az adott apphoz, például CRM connection. [🔗Link](https://learn.microsoft.com/en-us/power-platform/admin/pricing-billing-skus)

[Power Apps Premium listaár: €18.70 / user / hó](https://admin.microsoft.com/#/catalog/offer-details/power-apps-premium/D3F087F5-D71D-4930-A6F2-36E51743EFE4)

[Power Automate Premium listaár: €14.00 / user / hó](https://admin.microsoft.com/#/catalog/offer-details/power-automate-premium/738E8152-767C-4E17-AE67-7545C2D2EA67)  <- ne keverjük össze per userrel, ami ugyanennyibe kerül korlátozásokkal, kevesebb Dataverse capacityvel

A premium licencekre Dataverse kapacitás szempontból is szükség van, erről a következő fejezetben írok részletesen.

### Dataverse kapacitás és limitek

A premium licencek mellé 250 MB Dataverse DB capacity és 2 GB Dataverse File capacity jár, míg a per-app licenszek mellé 50 MB DB és 400 MB file. Mivel a dokumentumok tárolására Sharepointot tervezünk használni, a file capacity nem érdemes vizsgálni.

**Adatbázis kapacitás példa 1 dev + 20 userrel:** ~8.3 GB -> €126.7 / hó listaáron számolva

Mindenesetre a licencstratégia megválasztásakor az [admin centerben](https://admin.powerplatform.microsoft.com/billing/licenses/dataverse/overview) ellenőrizni kell a Capacity résznél, hogy a kiosztott per app tervek adta kvóta elegendő-e.

[Database extra capacity / 1 GB / hó](https://admin.microsoft.com/Adminportal/Home#/catalog/offer-details/dataverse-database-capacity-add-on/E331B1DE-6EF5-42BB-90F8-8266F7BBCD6E)

[Sharepoint szabad kapacitásunk: ~2.99 TB](https://axelhu-admin.sharepoint.com/_layouts/15/online/AdminHome.aspx#/siteManagement/view/ALL%20SITES)

### Microsoft 365 integráció

Ha a felhasználók rendelkeznek Microsoft 365 előfizetéssel, az megkönnyíti az olyan integrációkat, mint a Teams jóváhagyás. Ehhez nem kell külön Power Platform licenc, csak a meglévő Teams jogosultság. A licencstratégia tehát főképp a Power Apps és Automate használatra fókuszál.

### Licenszelés összefoglalás

A javasolt stratégiával a költségek kontrollálhatók, mivel nem kell minden usernek drágább korlátlan licencet adni, elegendő a célzott per app licenc. Ez a projekt tipikusan egy üzleti alkalmazásra szól, amihez a per app licenc ideális, a Microsoft is ezt javasolja ilyen méretű projektekhez. A fejlesztő / admin pedig megkapja a szükséges teljes körű hozzáférést a Premium licenccel, ami 1 fő esetén nem nagy tétel, viszont biztosítja, hogy bármi extra kell (pl. környezetek kezelése, custom connector, stb.) azt engedélyezze a rendszer.

Felül kell vizsgálni azonban:

- Ha az app mellé idővel szükség lesz egy külső portál (Power Pages) a partnerek számára, az a per app licenc keretben nem biztos hogy belefér, mert egy per app licenc vagy egy app vagy egy portál használatát engedi. Ha portált is akarnánk, lehet, hogy külön licencet kellene rá szánni, vagy más konstrukcióban gondolkodni. Ez most nincs tervben, de jövőbeli bővítésnél figyelendő.
- A fejlesztői környezet korlátja, hogy nem használható élesre. Ez benne van a Microsoft feltételeiben, productionra nem legális használni. Ezt betartjuk: külön éles környezet lesz megfelelő licencekkel. [🔗Link](https://learn.microsoft.com/en-us/power-platform/developer/plan)

Összességében ez a licencstratégia elegendő a megvalósításhoz. A Power Platform adminisztrátorok az admin centerben nyomon tudják követni a per app licencek kihasználtságát egy jelentésben, így monitorozható, hány licenc fogyott és ki használja. Ha a felhasználói kör bővülne, könnyen skálázható a modell további per app licencek vétele és kiosztása révén. [🔗Link](https://learn.microsoft.com/en-us/power-platform/admin/view-license-consumption-issues)

## Alkalmazás életciklus-kezelése (ALM) és verziókövetés

Célom, hogy a fejlesztéstől az éles üzemig kontrollált, ismételhető módon juttassuk el a megoldást, minimalizálva a hibalehetőségeket és biztosítva a változások nyomon követését. Személyes tapasztalat, hogy ennek hiánya mennyire meg tudja nehezíteni az alkalmazások utólagos módosítását, követését.

### Környezetstratégia

A Microsoft ajánlása, hogy legalább külön fejlesztői (Dev) és éles (Prod) környezetet használjunk, és lehetőség szerint legyen egy teszt/UAT környezet is a kettő között. [🔗Link](https://learn.microsoft.com/en-us/power-platform/alm/environment-strategy-alm)

Ennek megfelelően:

- A fejlesztés egy új **Dev** environmentben zajlik. Itt unmanaged módon dolgozunk a megoldáson.
- Opcionálisan létrehozunk egy **Test/UAT** környezetet, amelyben a kulcsfelhasználók jóváhagyhatják a fejlesztést élesítés előtt. Ez a környezet licenc szempontból lehet egy átmeneti sandbox, vagy az éles környezet egy sandbox klónja. A per app licenceket ide is át lehet mozgatni átmenetileg a tesztelőknek. Fontos, hogy a tesztkörnyezet konfigurációja (pl. kapcsolatok, csatlakozók) minél inkább utánozza az éleset, de éles adatokat még ne tartalmazzon. Ha migrálunk adatot, az csak anonimizált vagy részleges legyen a tesztben.
- Az éles **Prod** környezet a végleges használat helye, ide csak a letesztelt megoldás kerül fel.

**Megoldások kezelése:** Minden fejlesztést megoldásba (Solution) foglalunk, egy egyedi publisher prefix-szel (pl. RAS vagy ami illik a céghez). Így a test és prod környezetbe csak exportálni-importálni kell a megoldást. Nem végzünk közvetlen config változtatást élesben. Bármi módosítás kell, azt a Dev-ben tesszük meg, növeljük a megoldás verzióját, és újból telepítjük (managed) Prodba. Ez a megközelítés biztosítja a kontrollált frissítéseket és azt, hogy nincs félrekonfigurálás élesben. A Microsoft dokumentáció is kiemeli, hogy ne a Default Solution-ben fejlesszünk, hanem használjunk saját megoldásokat a testreszabásokhoz. [🔗Link](https://learn.microsoft.com/en-us/power-platform/alm/use-solutions-for-your-customizations#why-you-shouldnt-use-the-default-solutions-to-manage-customizations)

**Verziókezelés:** A fenti megoldás kezelés kikényszeríti a verziószám következetes növelését a további kiadásoknál. Ha van lehetőség, bevezetünk valamilyen forráskód-kezelést is: például az Azure DevOps Power Platform Build Tools használatát a megoldás exportálására és forráskód (YAML, HTML, XML) formában való tárolására. Így a verziók közti különbségek pontosan követhetők, és vész esetén visszaállítható egy korábbi állapot. Kezdetben, a rövid idő miatt, akár manuális exportokkal is lehet verziózni (pl. a megoldásfájlokat elmentjük), de hosszú távon érdemes automatizálni. Léteznek pipeline sablonok a Microsofttól, amelyekkel Dev -> Test -> Prod telepítések automatizálhatók (Power Platform Pipelines, GitHub Actions, Azure DevOps build pipeline). Ha van rá lehetőség, ezek bevezhetését is támogatnám. [🔗Link](https://learn.microsoft.com/en-us/power-platform/alm/)

**Konfigurációváltozások és paraméterezés:** A Power Platform ALM keretében használunk Environment Variable-eket minden olyan értékhez, ami környezetenként eltérhet (pl. e-mail címek a flow-ban, SharePoint site URL). Így a megoldás telepítésekor csak az adott környezet változóit kell beállítani, nem kell a folyamatokat szerkeszteni. Ez csökkenti az élesítéskori hibákat.

**Közös fejlesztés és devOps:** Ha többen fejlesztenek párhuzamosan, fontolóra vehetjük branching strategy alkalmazását is a megoldás kezelésen felül.

**Telepítési folyamat:** Mielőtt Prod-ba telepítjük a végleges verziót, készítünk egy teljes mentést az éles környezetben (ha volt ott előző verzió, vagy ha adatokat migráltunk). Az admin centerben lehetőség van az egész environment backupjára. Ha bármi gond lenne az új verzióval, legyen roll-back. A telepítés után azonnal végzünk egy sanity check-et az éles rendszeren is.

**Továbbfejlesztés és változáskezelés:** Miután éles az alkalmazás, bármilyen új CR jön, azokat gyűjtjük és ütemezzük a következő verziókba. Majd egy hotfix vagy minor release készül Dev-ben, ez megy át tesztelés után Prodba. A Microsoft ALM útmutatóiban több szcenárióra is van ajánlás - a mi projektünk a "Scenario 0: ALM for a new project" kategóriába esik, amit friss projekthez vázolnak, és később kibővíthető a DevOps automatizálással. [🔗Link](https://learn.microsoft.com/en-us/power-platform/alm/new-project-alm)

Végül, gondoskodunk arról is, hogy a megoldás dokumentációja naprakész legyen. Mind a technikai (entitások, flow-k leírása), mind a felhasználói dokumentáció a funkciókról. Ez része az ALM-nek, mert elősegíti a karbantarthatóságot és tudásátadást, pl. ha egy új fejlesztő csatlakozik később. [Dokumentáció sample](/samples/documentation-sample/)

[Saját Power Platform CLI notes ALM-hez](/samples/cli-notes-speti.md)

## Üzemeltetés, adminisztráció és megfelelőség

### Power Platform Admin Center szerepe

A **Power Platform Admin Center (PPAC)** központi szerepet játszik a megoldás életciklusának és napi működésének adminisztrációjában. Kulcsfeladatok, amiket a PPAC-en keresztül kezelünk:

- **Környezetek és licencek kezelése:** Itt hozzuk létre és konfiguráljuk a Dev-Test-Prod environmenteket. A PPAC felületén allokáljuk a megvásárolt Power Apps per app licencek kapacitását az adott éles környezethez, hogy a felhasználók használni tudják az appot. Ugyanitt nyomon követhetjük a licencfogyást a [License consumption reports](https://admin.powerplatform.microsoft.com/billing/licenses/dataverse/overview) alatt, és láthatjuk, mely felhasználók hány appot használnak.
- **Biztonság és hozzáférés:** Az admin centerben menedzseljük a környezet engedélyeit. Például beállítjuk, hogy ki legyen Environment Admin, Environment Maker, stb. Az éles környezetben tipikusan csak keveseknek lesz Admin joga. A végfelhasználók csak az app megosztásán keresztül kapnak hozzáférést a szükséges jogosultságokkal. Itt lehet felvenni felhasználókat, csoportokat és hozzárendelni nekik a létrehozott Dataverse szerepköröket is. Ez megtehető a Power Apps felületen is, de admin centerben átláthatóbb egyszerre.
- **DLP policy:** A PPAC-ben állíthatunk be Data Loss Prevention szabályokat tenant szinten vagy környezet szinten. Tekintve, hogy a szerződéskezelő érzékeny adatokat is tartalmaz, fontos, hogy a környezetben ne engedélyezzünk olyan connectorokat, amik veszélyeztethetik az adatszuverenitást. Például készítünk egy DLP szabályt, hogy a Dataverse és Office 365 connectorok engedélyezettek, de mondjuk a Twitter vagy Facebook vagy ismeretlen külső szolgáltatások connectorai tiltottak ezen a környezeten, így a flow-k nem vihetnek ki adatot nem biztonságos helyre. A DLP szabályokkal biztosítható, hogy a fejlesztők se tudjanak véletlenül nem engedélyezett irányba adatot küldeni. [🔗Link](https://learn.microsoft.com/en-us/power-platform/admin/prevent-data-loss)
- **Monitorozás és hibakeresés:** Az admin center Monitor szekciójában figyelemmel kísérhetjük a Dataverse használatot (tároló kapacitás, API hívások száma), a Power Automate flow-k futását (sikertelen futások száma, időzítések). Ha egy flow hibára fut, itt észrevehető. Szintén, a PPAC integrálódik a Power Platform Center of Excellence (CoE) kit megoldással, ami bővebb jelentéseket is adhat – adott esetben ezt be lehet vezetni a teljes szervezeti környezetre, de a mi projektre fókuszálva elég a beépített monitorozás. Az audit log kapcsán: a Microsoft Purview Audit Log szolgáltatáson keresztül a PPAC eseményei (pl. flow létrehozás, módosítás) is naplózódnak, és a compliance admin megnézheti ezeket, ha valami visszaélés gyanú van. [🔗Link](https://learn.microsoft.com/en-us/power-platform/admin/wp-compliance-data-privacy)
- **Biztonsági mentések:** Az admin center lehetőséget ad manuális vagy ütemezett backup készítésére az egész környezetről. Ezt felhasználjuk fontos mérföldköveknél, így bármilyen váratlan probléma esetén vissza tudunk térni egy előző állapothoz. A backup tartalmazza az adatokat is, ezért ezzel a GDPR előírásainak megfelelően kell bánni (titkosítottan tárolja a Microsoft, és csak adminok férnek hozzá).

### Adatvédelem, bizalmasság (GDPR) és megfelelőség

A szerződéskezelő rendszer személyes adatokat és üzleti bizalmas információkat is tartalmazhat, ezért a GDPR és egyéb adatvédelmi előírásoknak való megfelelés kritikus. A Power Platform választása sok szempontból segíti a megfelelést, de a konfiguráció és üzemeltetés során is figyelmet fordítunk erre:

**Adatkezelés helye és adatmozgatás:** A Dataverse környezet régiójának EU-t (azon belül North) választunk, így az adatok az EU-ban tárolódnak a Microsoft felhőjében. A Microsoft garantálja, hogy az adatok nem hagyják el a kiválasztott földrajzi régiót normál működés során, csak kivételes esetben, pl. támogatási folyamat során, és akkor is megfelelő jogi keretek között. Ezt támasztja alá a Microsoft Trust Center, ahol nyilvános információk vannak a Power Platform adatkezeléséről. [🔗Link](https://learn.microsoft.com/en-us/power-platform/admin/wp-compliance-data-privacy#trust-center)

**Titkosítás és biztonság:** A Dataverse minden adatot titkosít átvitel közben TLS-sel és nyugalmi állapotban automatikusan TDE-vel (Transparent Data Encryption) a szerveren. Ez azt jelenti, hogy ha illetéktelen hozzáférne is a fizikai adathordozóhoz, nem tudja elolvasni az adatokat. A Microsoft által menedzselt kulcsok megfelelő biztonságot nyújtanak, de lehetőség van saját kulcsot is használni (BYOK), bár ezt általában csak nagyon speciális igényeknél alkalmazzák. A platform rendszeresen auditálva van és megfelel több nemzetközi biztonsági szabványnak (ISO 27001, SOC, stb), ami garantálja, hogy technikailag erős védelem alatt állnak az adatok. [🔗Link](https://learn.microsoft.com/en-us/power-platform/admin/wp-compliance-data-privacy#data-protection)

**Hozzáférés korlátozása:** A role-based modellel biztosítjuk, hogy személyes adathoz csak az fér hozzá, akinek a feladata ezt indokolja.

**Adatmegőrzés és törlés:** A GDPR előírja, hogy személyes adatokat csak szükséges ideig őrizzünk. Ki kell dolgozni a szerződésekre egy megőrzési szabályt: pl. szerződés lejárta vagy megszűnése után X évvel archiválás vagy törlés. A rendszerben ezt támogathatjuk archiváló folyamatokkal (pl. évente egyszer a régi, már nem szükséges szerződéseket kiexportáljuk és töröljük). A Dataverse-ből való törlés esetén figyelni kell arra, hogy a biztonsági másolatokban még megmaradhat adat. Ha valakit törölni kell, mert GDPR-ra hivatkozva ezt kéri, akkor végrehajtjuk a Dataverse rekordok törlését és szükség esetén a backupból is 90 napon túl eltűnik (vagy explicit kérjük a Microsoft-ot a teljes törlésre).

**DSR kérések támogatása:** A Microsoft rendelkezésre bocsát útmutatókat, hogyan támogassuk a Data Subject Request-eket a Power Platformban. Például, ha egy érintett kéri az adatai exportját a rendszerből, a Dataverse adatok könnyen kinyerhetők Excelbe vagy PDF jelentésbe.

**Nyomon követhetőség és naplózás:** A beépített audit log és a Purview Compliance Manager segíthet egy esetleges incidens vizsgálatában. Az admin centerben engedélyezve lesz a Audit Log Search a Power Automate és Power Apps eseményekre is, így látható például, ha valaki exportált nagy mennyiségű adatot, vagy ha létrehozott egy új flow-t, ami gyanús lehet. Az Azure AD oldalon pedig nyomon követhető, ki és mikor lépett be a rendszerbe. Ezek az eszközök mind hozzájárulnak a biztonságos üzemhez.

**Megfelelőségi keretrendszer:** A Power Platform a Microsoft megfelelőségi keretrendszerébe tartozik, amely tartalmaz GDPR, ISO és egyéb megfelelőségi tanúsítványokat. A Microsoft Trust Center-ben dokumentált, hogy a Dataverse hogyan felel meg a GDPR-nak, ami megnyugtató alapot ad. Nekünk, mint megvalósítóknak arra kell ügyelni, hogy a saját adatkezelési folyamataink (pl. hozzáférés-kezelés, törlési folyamat) is megfeleljenek ennek. [🔗Link](https://www.microsoft.com/hu-hu/trust-center)

## Összefoglalás

A fent bemutatott terv egy átfogó, moduláris és bővíthető megoldást vázol az xFlower szerződéskezelő kiváltására a Microsoft Power Platform segítségével. Az architektúra középpontjában a Dataverse áll, amely robusztus adatkezelést és biztonságot nyújt, míg a Power Apps egy testreszabott, de gyorsan kialakítható felhasználói felületet biztosít a működéshez. A Power Automate a háttérben automatizálja a munkafolyamatokat, legyen szó jóváhagyásokról, értesítésekről vagy integrációkról, biztosítva, hogy a folyamatok hatékonyan történjenek.

A terv kitér az összes fontos szempontra:

- A folyamatok leképezése a platform eszközeivel, azonos működést garantálva az új környezetben.
- Az architektúra és komponensek szétválasztása oly módon, hogy a rendszer rugalmasan bővíthető és karbantartható legyen. Például új entitás hozzáadása vagy új folyamat beillesztése könnyen megtehető a meglévő keretek közé.
- Licencelési megfontolások, amelyekkel a költségek és a funkcionalitás egyensúlyban tartatók. A per app modell elegendő a jelen igényekre.
- A fejlesztés ütemezése reális keretek között, lehetővé téve már néhány hét után egy működő prototípus bemutatását, és kb. 3 hónapon belül a teljes rendszer éles használatba vételét.
- ALM és üzemeltetési elvek, melyek biztosítják, hogy a megoldás hosszú távon is fenntartható, verziói követhetők, a változtatások kontrolláltan kerülnek élesbe, a környezetek megfelelően el vannak szeparálva a különböző céltól: fejlesztés / éles használat
- Adatbiztonság és megfelelőség, tehát a rendszer nem csak funkcionálisan, de governance szempontból is megbízható - beépítve a szükséges biztonsági és adatvédelmi mechanizmusokat, auditálható működéssel.

A Microsoft által nyújtott hivatalos dokumentációkra és best practice-ekre támaszkodtam a terv elkészítése során – ezek alátámasztják a választott megoldás technikai helyességét és ajánlását. Például a több környezetes ALM modell, a megoldáscsomagok használata, a Dataverse biztonsági modell alkalmazása mind megfelel a Microsoft ajánlásainak. A végeredmény egy modern szerződéskezelő rendszer lesz, ami integrálódik a szervezet ökoszisztémájába, javítja a folyamatok átláthatóságát és hatékonyságát, és platformot ad a további digitális fejlesztéseknek.

## Források és hivatkozások

> in-line hivatkozások mellett

- xFlower meglévő rendszer dokumentáció – a jelenlegi folyamatok leírása, amelyek alapján történt a megfeleltetés

Microsoft Official

- [Microsoft Learn dokumentációk](https://learn.microsoft.com/en-us/), PL-200, PL-400 és PL-600 hivatalos oktatóanyagok
- [Microsoft Power Platform Licensing Guide](https://go.microsoft.com/fwlink/?linkid=2085130)
- [Microsoft Trust Center](https://www.microsoft.com/hu-hu/trust-center) és Compliance dokumentumok
- [MS Power Platform YouTube](https://www.youtube.com/@mspowerplatform)

Microsoft Partners / MVPs

- [Star Knowledge Blog](https://star-knowledge.com/blog/what-is-microsoft-dataverse-a-comprehensive-guide/)
- [MAQ Software](https://maqsoftware.com/insights/dataverse-security-best-practices)
- [DamoBird365 YouTube](https://www.youtube.com/@DamoBird365)
- [Sean Astrakhan YouTube](https://www.youtube.com/@Untethered365)
- [Shane Young YouTube](https://www.youtube.com/@ShanesCows)
- [Anders Jensen YouTube](https://www.youtube.com/@andersjensenorg)
