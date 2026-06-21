# Követelmények és Használati Esetek Igazolása

Ez a dokumentum a specifikációban ("specification.md") felsorolt követelmények és használati esetek tételes teljesülését bizonyítja a projekt forráskódja és játékmechanikái alapján.

## 1. Követelmények (Requirements) Igazolása

| Azonosító | Követelmény                                                                                             | Prioritás | Teljesülés    | Indoklás / Bizonyíték                                                                                                                                                                                         |
| :-------- | :------------------------------------------------------------------------------------------------------ | :-------- | :------------ | :------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **K1**    | A játékos a megfelelő billentyűk leütésével, mozgatni tudja a karaktert                                 | Kritikus  | **Teljesült** | A `PlayerController.cs` szkript feldolgozza az `Bal nyíl/A`, `Jobb nyíl/D` és `Space` billentyűket, és ezek alapján módosítja a karakter `Rigidbody2D` sebességét.                                            |
| **K2**    | A terep és az ellenfelek megfelelően jelenjenek meg és mozogjanak                                       | Kritikus  | **Teljesült** | A pályák (pl. `EnemyLevel.unity`) beépített Tilemap-okat használnak, az ellenfelek (pl. `PatrollingEnemy.cs`) pedig kijelölt útvonalakon (`pointA`, `pointB`) mozognak a fizikán keresztül.                   |
| **K3**    | A tereptárgyakkal (falak, talaj, platformok) megfelelően érintkezzen a karakterünk, és az ellenfelek is | Kritikus  | **Teljesült** | Minden aktív játékelem tartalmaz `Rigidbody2D` és `BoxCollider2D` komponenseket, valamint a játékos külön ellenőrzi a földetérést (`grounded` állapot).                                                       |
| **K4**    | A karakter és az ellenfelek érintkezése után a megfelelő fél szenvedjen el sebzést                      | Kritikus  | **Teljesült** | Az `EnemyAttackRange.cs` ütközésvizsgálattal kezdeményezi az ellenfél támadását, a játékos pedig a `Stomp` (ráugrás) mechanikán keresztül sebezheti az ellenséget. Mindkét esetben csökken a célpont HP-ja.   |
| **K5**    | A karakter legyen képes végrehajtani a speciális mozgásokat pl.: gyors mozgás, tölthető ugrás           | Magas     | **Teljesült** | A `PlayerController.cs`-ben implementálva van egy "charge jump" rendszer, ahol a Space nyomva tartásának idejétől függ az ugrás nagysága (ehhez vizuális squash/stretch torzulás is társul).                  |
| **K6**    | A játék végét egy "Boss fight"-tal kell lezárni                                                         | Magas     | **Teljesült** | A `BossScene.unity` jelenetben található a `BossSpawner` (`boss_shoot.cs`), amely saját támadási mintákkal és lövedékekkel (legyező alakban lő) rendelkező sárkányként (Maldrak) funkcionál.                  |
| **K7**    | A játékosnak logikai akadályokon is túl kelljen jutni a tovább haladáshoz                               | Közepes   | **Teljesült** | A `PuzzleLvL.unity` jelenetben a játékosnak dobozokat kell a nyomáskapcsolókra (`PressurePlate2D.cs`) tolnia, és karokat (`LeverSwitch2D.cs`) kell használnia a zárt ajtók és mozgó platformok aktiválásához. |
| **K8**    | A játék során a játékos ismerje meg a történetet                                                        | Közepes   | **Teljesült** | Az `Intro.unity` és `Outro.unity` jelenetekben a `CutsceneManager.cs` egy diavetítés (slideshow) formájában meséli el a királylány elrablásának és megmentésének történetét.                                  |
| **K9**    | A játék során játsszon le az éppen bekövetkező eseményekhez kapcsolódó hangokat                         | Alacsony  | **Teljesült** | A `MusicManager.cs` kezeli a jelenetek háttérzenéjét, míg a kapcsolók, ajtók és a Boss lövedékei saját `AudioSource` példányokon keresztül játszák le a hanghatásokat.                                        |

---

## 2. Használati Esetek (Use Cases) Igazolása

| Eset (ID)                           | Leírás                             | Teljesülés    | Indoklás                                                                                                                           |
| :---------------------------------- | :--------------------------------- | :------------ | :--------------------------------------------------------------------------------------------------------------------------------- |
| **U1: Játék indítása**              | Játék elindítása a menüből         | **Teljesült** | A `TempMainMenu.unity` jelenetben a Play gomb betölti az Intro-t, a `MainMenu.PlayGame()` metóduson keresztül.                     |
| **U2: Karakter mozgatás**           | Főkarakter mozgása                 | **Teljesült** | A játékos az A/D gombokkal irányíthatja a karakterét a beállított tengelyeken.                                                     |
| **U3: Újraindítás**                 | Játékmenet újraindítása halál után | **Teljesült** | A halál után megjelenő `GameOver` képernyőn a Restart gombra kattintva, vagy az `R` gombbal (`ResetManager.cs`) a pálya újraindul. |
| **U4: Kilépés a játékból**          | Kilépés                            | **Teljesült** | A főmenüben lévő Exit gomb meghívja az `Application.Quit()` függvényt.                                                             |
| **U5: Ellenség interakciók**        | Életerő vesztés és támadás         | **Teljesült** | A karakter belehalhat az ellenfél (Mágus vagy Toronyőr) támadásaiba, de felülről rájuk ugorva el is tudja pusztítani őket.         |
| **U6: Terep interakciók**           | Tárgyakkal való interakció         | **Teljesült** | A játékos az 'E' gombbal aktiválhatja az urnákat (`GoldUrnInteractable`), és a karokat.                                            |
| **U7: Ugrás**                       | A főhős horizontális mozgása       | **Teljesült** | A Space lenyomásával a karakter `Rigidbody2D` sebességhez jut az Y tengelyen.                                                      |
| **U8: Jobbra-balra mozgás**         | Vízszintes mozgatás                | **Teljesült** | Vízszintes tengelyes (X tengely) input beolvasásával.                                                                              |
| **U9: Speciális mozgások**          | Tölthető ugrás stb.                | **Teljesült** | Space nyomva tartásával tölthető ugrás van implementálva, amely vizuálisan és fizikálisan is módosítja az ugrást.                  |
| **U10: Szintek teljesítése**        | Következő szintre lépés            | **Teljesült** | A pályák szélén lévő `SceneEdgeTransitionTrigger` átdobja a játékost a következő pályára.                                          |
| **U11: Puzzle megoldása**           | A szinten lévő puzzle              | **Teljesült** | Dobozok tologatása a nyomáskapcsolókra a megfelelő ajtók kinyitásához.                                                             |
| **U12: Életpont vesztés**           | Sebződés                           | **Teljesült** | A tüskékbe (`SpikeHitbox.cs`) lépés vagy az ellenfelek találata levon egyet az UI életblokkokból.                                  |
| **U13: Visszaugrás mentési pontra** | Respawn                            | **Teljesült** | A `ResetManager.ResetScene` metódus segítségével a játékos újrakezdheti az adott pályarészt.                                       |
| **U14: Mentési pont frissítése**    | Checkpoint elmentése               | **Teljesült** | Minden pálya elhagyása után a `ResetManager` az új pálya kezdeti állapotát tölti vissza.                                           |
| **U15: Boss fight**                 | Végső csata                        | **Teljesült** | A 3. pályán lévő sárkány legyőzése (életkristályokkal és lövedék-alapú támadásokkal).                                              |
| **U16: Sztori befejezése**          | Küldetés teljesítése               | **Teljesült** | A Boss halála után az `Outro` jelenet betöltődik, ahol a történet véget ér.                                                        |

---

## 3. Új Követelmények és Használati Esetek (Opcionális)

A projekt fejlesztése során az alap specifikációban nem szereplő, új rendszerek kerültek kidolgozásra a játékélmény javítása érdekében: a **Pontrendszer (Score)** és a **Toplista (Leaderboard)**.

### Új Követelmények

| Azonosító    | Követelmény                                                                                                    | Prioritás |
| :----------- | :------------------------------------------------------------------------------------------------------------- | :-------- |
| **K10 (Új)** | A játékos a pályán található ellenfelek elpusztításáért és elrejtett aranyurnák széttöréséért pontokat kapjon. | Magas     |
| **K11 (Új)** | A játék végén a játékos elmenthesse a nevét és pontszámát egy perzisztens toplistára (Leaderboard).            | Közepes   |

### Új Használati Esetek

| Eset                            | Leírás                                                                                             | Prioritás |
| :------------------------------ | :------------------------------------------------------------------------------------------------- | :-------- |
| **Pontszám Gyűjtése**           | A játékos a felvehető/elpusztítható objektumokkal növeli az összpontszámát.                        | Magas     |
| **Eredmény Mentése (Toplista)** | A Boss legyőzése után a `GameWinHandler` segítségével beírja a nevét a helyi Highscore rendszerbe. | Közepes   |
