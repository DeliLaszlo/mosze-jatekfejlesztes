# Aetheria - Szoftverfejlesztési Specifikáció

**Metaadatok:**

- **Játék neve:** Aetheria
- **Engine:** Unity
- **Programozási nyelv:** C#
- **Készítette:** Ludván Attila, László Viktor Deli, Németh Patrik, Szigetvári Szabolcs
- **Dátum:** 2026.03.10
- **Intézmény:** Széchenyi Egyetem (University of Győr)
- **Tárgy:** Modern szoftverfejlesztési eszközök (GKNB_INTM006)

---

## Tartalomjegyzék

- [1. Bevezetés](#1-bevezetés)
- [1.1 A dokumentum célja](#11-a-dokumentum-célja)
- [1.2 Projekt áttekintése](#12-projekt-áttekintése)
- [1.3 Gondolattérkép](#13-gondolattérkép)
- [2. A játék koncepciója](#2-a-játék-koncepciója)
- [2.1 A játék ötlete](#21-a-játék-ötlete)
- [2.2 Játék műfaja](#22-játék-műfaja)
- [2.3 Célközönség](#23-célközönség)
- [3. Történet](#3-történet)
- [3.1 A játék világa](#31-a-játék-világa)
- [3.2 Történet](#32-történet)
- [3.3 A fő karakter](#33-a-fő-karakter)
- [3.4 Ellenségek](#34-ellenségek)
- [4. Követelmények](#4-követelmények)
- [5. Használati esetek](#5-használati-esetek)
- [6. Struktúra](#6-struktúra)
- [7. Verziókezelés](#7-verziókezelés)
- [8. Játékmenet](#8-játékmenet)
- [8.1 A játék célja](#81-a-játék-célja)
- [8.2 Játék mechanikák](#82-játék-mechanikák)
- [9. Irányítás](#9-irányítás)
- [10. Pályatervezés](#10-pályatervezés)
- [11. Karakterek](#11-karakterek)
- [12. Grafikai stílus](#12-grafikai-stílus)
- [13. Hangok és zene](#13-hangok-és-zene)
- [14. Felhasználói felület](#14-felhasználói-felület)
- [15. Technikai megvalósítás](#15-technikai-megvalósítás)
- [16. Adatkezelés](#16-adatkezelés)
- [17. Tesztelés](#17-tesztelés)
- [18. Fejlesztési ütemterv](#18-fejlesztési-ütemterv)
- [19. Összegzés](#19-összegzés)

---

## 1. Bevezetés

A számítógépes játékok fejlesztése napjainkban egyre népszerűbb terület az informatikában, mivel ötvözi a programozást, a kreatív tervezést és a grafikai megvalósítást. A játékfejlesztés során fontos szerepet kap a megfelelő tervezés és dokumentáció, amely segíti a fejlesztési folyamat átláthatóságát és a projekt szervezett megvalósítását.

A jelen dokumentum célja egy Unity játékmotorral készülő játék teljes specifikációjának bemutatása. A dokumentum részletesen ismerteti a játék koncepcióját, történetét, játékmenetét, karaktereit, valamint a technikai megvalósítás főbb elemeit. Emellett bemutatja a fejlesztési folyamat során alkalmazott eszközöket, például a verziókezelést és a dokumentációs módszereket.

A fejlesztett játék egy platformer stílusú kalandjáték, amelyben a játékos feladata, hogy egy több szintből álló torony tetejére jusson, ahol a királylány várja. A játék során a játékos különböző akadályokat, platformokat és ellenségeket kerül ki vagy győz le, miközben egyre magasabb szintekre jut a toronyban. A játék a Unity játékmotor segítségével készül, C# programozási nyelv használatával. A projekt célja egy olyan játék létrehozása, amely egyszerű alapmechanikákkal rendelkezik, ugyanakkor kihívást és szórakozást nyújt a játékos számára. A dokumentum célja továbbá, hogy átfogó képet adjon a projekt felépítéséről és működéséről, valamint segítséget nyújtson a játék fejlesztési folyamatának megértéséhez.

### 1.1 A dokumentum célja

A dokumentum célja a Aetheria nevű játék teljes tervezési és technikai specifikációjának bemutatása. A dokumentum részletesen ismerteti a játék működését, játékmenetét, grafikai megoldásait, valamint a fejlesztés során alkalmazott technológiákat. A specifikáció segítségével a fejlesztők és a projekt résztvevői átfogó képet kapnak a játék felépítéséről és működéséről. A dokumentum tartalmazza a játékmenet leírását, a pályatervezési szempontokat, a karakterek tulajdonságait, valamint a technikai megvalósítás részleteit a Unity játékmotor használatával.

### 1.2 Projekt áttekintése

Az Aetheria egy akció-kaland stílusú játék, amelyben a játékos egy karaktert irányít, aki különböző pályákon keresztül próbál eljutni a célba, miközben akadályokat kerül ki és ellenségekkel harcol. A játék célja, hogy a játékos ügyességét és reakcióidejét tesztelje. A játék Unity motor segítségével készül, amely lehetővé teszi a gyors fejlesztést és a hatékony grafikai megjelenítést.

### 1.3 Gondolattérkép

TODO: Gondolattérkép beillesztése

---

## 2. A játék koncepciója

### 2.1 A játék ötlete

A játék alapötlete egy gyors tempójú akciójáték létrehozása, amelyben a játékosnak különböző akadályokon kell áthaladnia és ellenségeket kell legyőznie. A játék inspirációját több klasszikus platformer játék adta, ahol a gyors mozgás és a pontos időzítés kulcsfontosságú. A cél egy olyan játék létrehozása volt, amely egyszerűen megtanulható, ugyanakkor kihívást jelent a játékos számára.

### 2.2 Játék műfaja

A játék műfaja:

- Akció
- Platformer
- Kaland

Ezek a műfajok kombinációja lehetővé teszi, hogy a játékos egyszerre élvezze a gyors tempójú akciót és a felfedezés élményét.

### 2.3 Célközönség

A játék célközönsége elsősorban a fiatal játékosok és azok, akik szeretik az egyszerű, de kihívást jelentő játékokat. Ajánlott korosztály: 12 év felett. A játék könnyen megtanulható irányítással rendelkezik, így kezdő játékosok számára is megfelelő.

---

## 3. Történet

### 3.1 A játék világa

A játék egy fantasy világban játszódik, amelyet Aetheria Királyságának hívnak. A királyság sokáig békében élt, és a lakók boldogan éltek a király uralma alatt. A királyság közepén áll egy hatalmas, ősi torony, amelyet Az Őrzők Tornyának neveznek. A legenda szerint a torony a világ egyik legerősebb mágikus helye. A torony tetején található egy ősi ereklye, amely hatalmas energiát tartalmaz.

### 3.2 Történet

Egy nap egy gonosz varázsló, Maldrak, megtámadja a királyságot. Maldrak célja, hogy megszerezze a torony tetején lévő mágikus erőt, amellyel uralma alá hajthatná az egész világot. A varázsló elrabolja a királylányt, és felviszi őt a torony legfelső szintjére. A királylányt csaliként használja, hogy senki ne merjen a toronyba lépni. A torony tele van csapdákkal, szörnyekkel és különböző akadályokkal, amelyeket Maldrak azért helyezett el, hogy megállítsa azokat, akik megpróbálnák megmenteni a királylányt.

### 3.3 A fő karakter

A játék főhőse egy bátor kalandor, aki meghallja a királylány elrablásának hírét. Elhatározza, hogy megmenti őt, és elindul a torony felé. A torony azonban nem egyszerű épület. Minden szint új kihívásokat tartogat, például:

- mozgó platformokat
- veszélyes csapdákat
- ellenséges lényeket
- rejtett átjárókat

A játékosnak ügyességre és gyors reakciókra lesz szüksége ahhoz, hogy egyre magasabbra jusson a toronyban.

### 3.4 Ellenségek

A játék során a játékos különböző ellenségekkel találkozik. Például:

- Toronyőr
- Tüskés lény
- Maldrak

Ezek az ellenségek megpróbálják megállítani a játékost.

---

## 4. Követelmények

| Azonosító | Követelmény                                                                                             | Prioritás |
| :-------- | :------------------------------------------------------------------------------------------------------ | :-------- |
| **K1**    | A játékos a megfelelő billentyűk leütésével, mozgatni tudja a karaktert                                 | Kritikus  |
| **K2**    | A terep és az ellenfelek megfelelően jelenjenek meg és mozogjanak                                       | Kritikus  |
| **K3**    | A tereptárgyakkal (falak, talaj, platformok) megfelelően érintkezzen a karakterünk, és az ellenfelek is | Kritikus  |
| **K4**    | A karakter és az ellenfelek érintkezése után a megfelelő fél szenvedjen el sebzést                      | Kritikus  |
| **K5**    | A karakter legyen képes végrehajtani a speciális mozgásokat pl.: gyors mozgás, tölthető ugrás           | Magas     |
| **K6**    | A játék végét egy "Boss fight"-tal kell lezárni                                                         | Magas     |
| **K7**    | A játékosnak logikai akadályokon is túl kelljen jutni a tovább haladáshoz                               | Közepes   |
| **K8**    | A játék során a játékos ismerje meg a történetet                                                        | Közepes   |
| **K9**    | A játék során játsszon le az éppen bekövetkező eseményekhez kapcsolódó hangokat                         | Alacsony  |

---

## 5. Használati esetek

| Eset                                        | Leírás                                                     | Prioritás |
| :------------------------------------------ | :--------------------------------------------------------- | :-------- |
| **Játék indítása**                          | -                                                          | Magas     |
| **Karakter mozgatás**                       | Főkarakter mozgása                                         | Magas     |
| **Újraindítás**                             | Játékmenet újraindítása halál után                         | Közepes   |
| **Kilépés a játékból**                      | -                                                          | Közepes   |
| **Ellenség interakciók**                    | A főhős életerőt veszít, az ellenségeket megtámadja        | Közepes   |
| **Terep interakciók**                       | A pályán lévő tárgyakkal való interakció                   | Magas     |
| **Ugrás**                                   | A főhős horizontális mozgása                               | Magas     |
| **Jobbra-balra mozgás**                     | A főhős vízszintes mozgatása                               | Magas     |
| **Speciális mozgások**                      | -                                                          | Közepes   |
| **Szintek teljesítése**                     | Az aktuális szint teljesítése és a következő szintre lépés | Magas     |
| **Puzzle megoldása**                        | A szinten található puzzle megoldása                       | Magas     |
| **Életpont vesztés**                        | A főhős életpontjából veszít                               | Közepes   |
| **Visszaugrás legutóbbi a mentési pontra**  | -                                                          | Alacsony  |
| **Mentési pont frissítése**                 | -                                                          | Alacsony  |
| **Boss fight**                              | A főhős csatája a végső ellenféllel                        | Magas     |
| **Sztori befejezése, Küldetés teljesítése** | -                                                          | Magas     |

TODO: Use Case diagram beillesztése

---

## 6. Struktúra

TODO: Folyamatábra beillesztése

---

## 7. Verziókezelés

A projekt fejlesztése során verziókezelő rendszert használunk annak érdekében, hogy a projekt fájljainak módosításai nyomon követhetőek legyenek. A verziókezelés lehetővé teszi, hogy a fejlesztők biztonságosan dolgozzanak ugyanazon a projekten, miközben minden változtatás rögzítésre kerül. A projekt esetében a Git verziókezelő rendszert és a GitHub online platformot használjuk. A Git lehetővé teszi a projekt teljes történetének követését, így minden módosítás visszakereshető és szükség esetén visszaállítható egy korábbi állapot.

A projekt teljes Unity mappája egy GitHub repository-ban kerül tárolásra. A repository tartalmazza a játékhoz szükséges összes fájlt, például a script fájlokat, a scene fájlokat, a grafikai elemeket, valamint a dokumentációt. A fejlesztés során a fejlesztők rendszeresen commitokat készítenek. A commit egy mentési pont a projekt történetében, amely tartalmazza az adott változtatásokat és egy rövid leírást arról, hogy milyen módosítás történt. A GitHub használatával a projekt minden résztvevője hozzáférhet a legfrissebb verzióhoz, és könnyen együtt tudnak dolgozni a fejlesztés során. Emellett a verziókezelés lehetővé teszi, hogy a projekt korábbi verzióit visszaállítsuk, ha egy új módosítás hibát okozna. A verziókezelés alkalmazása jelentősen növeli a fejlesztési folyamat biztonságát és átláthatóságát.

---

## 8. Játékmenet

### 8.1 A játék célja

A játék célja, hogy a játékos eljusson a pálya végére, és legyőzze a gonosz varázslót.

### 8.2 Játék mechanikák

A játék több alapvető mechanikát tartalmaz:

- **Mozgás:** A karakter képes ugrani, amit a space gomb lenyomva tartásával tud tölteni, hogy magasabbra ugorjon.
- **Ugrás:** A játékos átugorhat akadályokat.

---

## 9. Irányítás

| Billentyű | Funkció |
| :-------- | :------ |
| **A**     | balra   |
| **D**     | jobbra  |
| **Space** | ugrás   |
| **Esc**   | pause   |

---

## 10. Pályatervezés

A játék több pályából áll. Pálya típusok:

1.  Ellenséges pálya
2.  Puzzle pálya
3.  Boss pálya

A pályák nehézsége fokozatosan növekszik.

---

## 11. Karakterek

**Fő karakter**
Tulajdonságai:

- gyors mozgás
- tölthető ugrás

**Ellenségek**
Az ellenségek különböző viselkedéssel rendelkeznek. Például:

- járőröző toronyőrök
- járőröző tüskés lények

---

## 12. Grafikai stílus

A játék 2D grafikai stílust használ. A vizuális stílus pixel art. Ez lehetővé teszi a jó teljesítményt és a letisztult megjelenést.

---

## 13. Hangok és zene

A játék hangrendszere több elemből áll.

- **Háttérzene:** Dinamikus zene, amely a játékmenethez igazodik.
- **Hang effektek:**
  - ugrás
  - ellenség támadás

---

## 14. Felhasználói felület

A felhasználói felület egyszerű és könnyen érthető.

- **Főmenü:**
  - Start Game
  - Settings
  - Exit
- **Játék közbeni UI:**
  - élet
  - idő

---

## 15. Technikai megvalósítás

A játék fejlesztése a Unity motor segítségével történik.

**Használt rendszerek:**

- Rigidbody
- Collider
- Animator
- Unity UI

**Programozási nyelv:**
A játék C# nyelven írt scripteket használ.

---

## 16. Adatkezelés

A játék képes menteni a játékos pontszámát és eredményeit. A mentések PlayerPrefs segítségével történnek.

---

## 17. Tesztelés

A fejlesztés során több tesztelési módszert alkalmazunk.

- játékmenet teszt
- hibakeresés
- teljesítmény teszt

---

## 18. Fejlesztési ütemterv

| Fázis             | Időtartam |
| :---------------- | :-------- |
| **Tervezés**      | 1 hét     |
| **Prototípus**    | 1 hét     |
| **Fejlesztés**    | 7 hét     |
| **Tesztelés**     | 2 hét     |
| **Hibajavítások** | 2 hét     |
| **Végső tesztek** | 2 hét     |

---

## 19. Összegzés

A dokumentumban bemutatásra került a fejlesztés alatt álló Unity alapú játék teljes tervezési és technikai specifikációja. A játék egy klasszikus platformer stílusú kalandjáték, amelyben a játékos célja, hogy felmásszon egy több szintből álló torony tetejére, ahol a királylány várja. A játék során a játékos különböző akadályokon, platformokon és ellenségeken keresztül halad, miközben egyre nehezebb kihívásokkal találkozik.

A dokumentum részletesen ismertette a játék történetét, a játékmenet alapmechanikáit, a pályák felépítését, valamint az ellenségek különböző típusait. A játék során a játékos ügyességére és reakcióidejére van szükség, hogy sikeresen haladjon előre a torony szintjein. A torony minden szintje új akadályokat és ellenfeleket tartalmaz, amelyek fokozatosan növelik a játék nehézségét.

A projekt fejlesztése a Unity játékmotor segítségével történik, C# programozási nyelv használatával. A játék különböző Unity rendszereket alkalmaz, például fizikát, animációkat és felhasználói felületet, amelyek biztosítják a játékmenet megfelelő működését. A fejlesztési folyamat során fontos szerepet kap a dokumentáció és a verziókezelés. A projekt fájljai GitHub repository-ban kerülnek tárolásra, amely lehetővé teszi a változtatások nyomon követését és a projekt különböző verzióinak kezelését. Ez biztosítja a fejlesztési folyamat átláthatóságát és segíti a hibák gyors javítását.

Összességében a projekt célja egy szórakoztató és kihívást jelentő játék létrehozása, amely klasszikus platformer elemekre épül. A játék a torony megmászására és a királylány megmentésére fókuszál, miközben fokozatosan növekvő nehézséget és változatos játékmenetet kínál a játékos számára.
