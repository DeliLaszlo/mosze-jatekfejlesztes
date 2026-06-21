# Aetheria Játék Dokumentáció (Aetheria Game Documentation)

Ez a fájl az Aetheria játék részletes, produkciós szintű (production-ready) dokumentációját tartalmazza. A dokumentáció kitér a teljes projektstruktúrára, a jelenetekre, az összes kulcsfontosságú szkriptre, azok működésére és kapcsolataira.

---

## 1. Projekt Struktúra (`Assets/`)

A projekt az `Assets/` mappában az alábbi főbb könyvtárakra oszlik:

- **Audio**: Hangeffektek és zenék tárolója. Ezeket használja a `MusicManager` és a `SoundMixerManager`.
- **BossAssets / CostumeAssets / Images / MedievalPixelArtAssets / PaperMenu**: A játék 2D-s grafikai elemei, sprite-ok, textúrák, UI elemek és környezeti assetek.
- **MageEnemy / PatrollingEnemy / Player**: Az egyes karakterek és ellenségek saját mappái, amelyek tartalmazzák a hozzájuk tartozó kódot, animációkat és prefabokat.
- **Materials**: Különféle renderelési anyagok (materials) és shaderek gyűjteménye.
- **Prefabs**: Újrahasználható játékobjektumok (pl. `Camera.prefab`, `Door.prefab`, `GoldUrn.prefab`, `PauseMenu.prefab`, `ScreenFadeCanvas.prefab`).
- **Scenes**: A játék különböző pályái és menüi (`BossScene`, `EnemyLevel`, `Intro`, `Outro`, `PuzzleLvL`, `MainMenu`).
- **Scripts**: A globális játékmenet logikát, UI vezérlést, audio menedzsmentet, átmeneteket és rejtvényeket tartalmazó C# szkriptek.
- **Tests**: A projekt stabilitását garantáló NUnit alapú PlayMode és EditMode tesztek gyűjteménye.

---

## 2. Jelenetek (Scenes) Architektúrája

A játék menete több, egymással logikailag összekötött jelenetből áll:

- **MainMenu**: A főmenü, amely lehetőséget ad a játék indítására, a beállítások elérésére és a toplista megtekintésére.
- **Intro**: Bevezető jelenet egy állóképes slideshow-val (`CutsceneManager`), amely bemutatja a történetet, majd automatikusan áttölti a játék első pályáját.
- **PuzzleLvL**: Harcmentes, rejtvényekre koncentráló pálya (nyomáskapcsolók, mozgó platformok, ajtók), ahol a logikai interakció a lényeg.
- **EnemyLevel**: A fő harci jelenet. Itt kapnak helyet a járőröző ellenfelek, a Mágus, és a pontgyűjtő mechanikák (urnák).
- **BossScene**: Aréna jellegű végső küzdelem a Boss ellen, egyedi lövedék-mechanikákkal és mozgási mintákkal.
- **Outro**: Lezáró képsorozat, amely után a győzelmi képernyő (`GameWinHandler`) lehetővé teszi az eredmények toplistára mentését.

---

## 3. Player Scripts

### 3.1 `PlayerController`

**Fájl:** `Assets/Player/Scripts/PlayerController.cs`

**Feladat:**

- Játékosmozgás (bal/jobb), tölthető ugrás (charge jump), földkapcsolat ellenőrzése.
- Animator paraméterek frissítése és állapotok kényszerített szinkronja.
- Vizuális charge pose (squash/stretch, tint) kezelése az ugrás töltése közben.

**Fő viselkedés:**

- `Update()`: Bemenetek (input) olvasása, ugrás töltésének (charge) kezelése, ugrás indítása és animator paraméterek frissítése.
- `FixedUpdate()`: Fizikai mozgás alkalmazása (sebesség beállítása a `Rigidbody2D`-n), földetérés (grounded) állapot ellenőrzése.
- `ApplyFacing()`: Karakter vizuális forgatása a mozgásirány függvényében.
- `UpdateChargePose()`: A töltés közbeni vizuális torzítás (scale és szín) alkalmazása.

### 3.2 `PlayerHealthManager`

**Fájl:** `Assets/Player/Scripts/PlayerHealthManager.cs`

**Feladat:**

- Életpontok nyilvántartása, sebzés fogadása, halál állapot kezelése, és pálya-újraindítás vagy visszatöltés biztonságos kezelése.
- Életváltozás esemény (`HealthChanged`) publikálása az UI felé.

**Fő viselkedés:**

- `Awake()`: Max és aktuális health inicializálása, esetleg statikus memóriából visszatöltve.
- `TakeDamage()`: Életpontok csökkentése, sebzési animáció lejátszása és halál ellenőrzése.
- `HandleDeath()`: Animáció indítása, és minden input/fizika letiltása halál esetén.
- `ClearPersistedHealth()`: Statikus metódus, perzisztens életpontok nullázására (pl. főmenüből való újrakezdéskor).
- `OverrideNextSpawnHealth()`: Statikus metódus, amely lehetővé teszi, hogy újraspawnoláskor kevesebb HP-val kezdjen a játékos (pl. halál után).

### 3.3 `PlayerHealthBlocksUI`

**Fájl:** `Assets/Player/Scripts/PlayerHealthBlocksUI.cs`

**Feladat:**

- HP blokkos UI építése és frissítése dinamikusan a `PlayerHealthManager` aktuális állapota alapján.
- Színváltás és üres/teli blokkok kezelése életszinttől függően.

**Fő viselkedés:**

- `BuildBlocks()`: UI elemek (blokkok) dinamikus példányosítása és felépítése.
- `RefreshBlocks()`: Kitöltött és üres blokkok állapotának, valamint az aktív színek frissítése a HP változásakor.
- `EnsureHorizontalLayout()`: A blokkok elrendezésének automatikus beállítása a képernyőn.

---

## 4. Patrolling Enemy Scripts

### 4.1 `PatrollingEnemy`

**Fájl:** `Assets/PatrollingEnemy/Scripts/PatrollingEnemy.cs`

**Feladat:**

- Két előre meghatározott pont (`pointA`, `pointB`) között folyamatos járőrözés.
- Támadási logika a játékos közelében.
- Játékos "stomp" (ráugrás) eseményének kezelése és a halál logika lefolytatása.

**Fő viselkedés:**

- `Start()`: Komponensek kikeresése, patrol határok inicializálása.
- `FixedUpdate()`: Vízszintes mozgás, és a határoknál automatikus irányváltás (sprite flip).
- `HandleAttackRangeTrigger()`: Játékos felderítése és támadási fázis indítása.
- `AttackRoutine()`: Támadási animáció lejátszása, majd sebzés küldése a játékosnak.
- `DieFromStomp()`: Halál animáció, pontok hozzáadása, collider/physics kikapcsolása.

**Fontos Inspector mezők:**

- Patrol: `pointA`, `pointB`, `moveSpeed`
- Attack: `attackTriggerName`, `attackDuration`, `destroyPlayerOnAttack`

### 4.2 `EnemyAttackRange` (HitboxHandler)

**Fájl:** `Assets/PatrollingEnemy/Scripts/AttackHitboxHandler.cs`

**Feladat:**

- Trigger zónában követi a játékost, és periodikusan újrajelenti az ellenfél tulajdonosnak, ha támadásra alkalmas távolságba ért.

**Fő viselkedés:**

- `OnTriggerEnter2D()` / `OnTriggerExit2D()`: Játékos felvétele/eltávolítása a követési listából.
- `MonitorPlayersInRange()`: Periodikus ellenőrzés, és a szülő `PatrollingEnemy` értesítése a támadásról.

### 4.3 `Stomp` (DeathByPlayer)

**Fájl:** `Assets/PatrollingEnemy/Scripts/DeathByPlayer.cs`

**Feladat:**

- Az ellenfél feje feletti hitbox, amelyen keresztül a játékos megsemmisítheti az ellenfelet ráugrással (Mario-stílus).

**Fő viselkedés:**

- `OnTriggerEnter2D()`: Ha a játékos lép be felülről, meghívja az ellenfél `DieFromStomp()` metódusát.

---

## 5. Mage Enemy Scripts

### 5.1 `MageEnemyController`

**Fájl:** `Assets/MageEnemy/Scripts/MageEnemyController.cs`

**Feladat:**

- Komplex, állapotgép (State Machine) alapú mágus ellenfél vezérlése (`Shielded`, `Attacking`, `Vulnerable`, `Teleporting`, `Dead`).
- Pajzs alatti sérthetetlenség, botcsapásos (Slam) területi támadás, és véletlenszerű teleportálás a pályán.
- Büntetés leosztása, ha pajzsos állapotban ugrik rá a játékos.

**Fő viselkedés:**

- `Start()`: HP beállítása, teleport pont index meghatározása, kezdés `Shielded` állapotban.
- `Update()`: Állapotonkénti időzítés és átmenetek (`Shielded` -> támadás, `Vulnerable` -> teleport).
- `StartAttack()`: Animáció indítása a Slam támadáshoz.
- `OnStaffHitGround()`: Animáció eventből hívva; Slam VFX lejátszása és sebzés (`ApplySlamDamage()`) kiosztása.
- `HandleTeleportSequence()`: Eltűnés VFX, láthatatlanság, pozícióváltás a teleport pontra, majd megjelenés VFX.
- `HandleStomp()`: Sérülékeny (Vulnerable) állapotban HP csökkentése, ha nem, akkor a játékos büntetése és visszateleportálása (`PunishPlayerSequence()`).
- `Die()`: Halál animáció, collider kikapcsolása, score kiosztása.

**Fontos Inspector mezők:**

- `maxHealth`, `timeBetweenAttacks`, `vulnerableDuration`, `slamHitbox`
- `teleportPoints`, `damageTeleportOutColor`
- VFX: `shieldParticles`, `slamParticles`, `teleportOutParticles`

### 5.2 `MageStompReciever`

**Fájl:** `Assets/MageEnemy/Scripts/MageStompReciever.cs`

**Feladat:**

- A mágus fején lévő hitbox, amely a Stomp eseményeket továbbítja a `MageEnemyController`-nek.

**Fő viselkedés:**

- `OnTriggerEnter2D()`: Játékos ütközése esetén meghívja a `HandleStomp(...)` logikát.

---

## 6. Boss Enemy Scripts

### 6.1 `BossSequence`

**Fájl:** `Assets/Scripts/BossSequence.cs`

**Feladat:**

- A végső Boss mozgási mintázatának kezelése.
- Nyolcas alakú (Figure-8) és háromszög alakú útvonalak végrehajtása fázisonként, köztük statikus várakozási idővel.

**Fő viselkedés:**

- `Update()`: A `currentState` alapján hívja a megfelelő mozgási metódust (`EightShape`, `Waiting`, `Triangle`).
- `MoveInEight()`: Szinusz és koszinusz függvényekkel nyolcas pályát ír le, beállított ismétlésszámig.
- `MoveInTriangle()`: Lerp-pel egy háromszög három pontja között mozgatja a Bosst.
- `GoToWait()`, `HandleWait()`: Várakozási fázis átmenete a mintázatok között.
- `FlipSprite()`: A haladási iránynak megfelelően megfordítja a vizuális modellt.

### 6.2 `BossSpawner` (`boss_shoot.cs`)

**Fájl:** `Assets/Scripts/boss_shoot.cs`

**Feladat:**

- A Boss harci logikája: lövedékek kilövése (Fan attack), sebzés fogadása, sebezhetetlenség (invulnerability), teleportálás találat esetén, valamint a győzelem és pálya átmenet kezelése.

**Fő viselkedés:**

- `BossLogicRoutine()` (Coroutine): Folyamatosan lő, amíg a `BossSequence.IsMoving()` igaz.
- `ShootFan()`: A játékos irányába kiszámol egy szöget, és aszerint legyező alakban (több lövedék) lő. A lövedékek száma a Boss életének csökkenésével dinamikusan nő. A kilőtt lövedékeket `SimpleBullet`-ként inicializálja.
- `TakeDamage()`: Életerő csökkentése, ha nem invulnerable. Találatkor meghívja a `HitRoutine()`-t.
- `HitRoutine()` (Coroutine): Ideiglenes villogás (sprite ki/be kapcsolása), ami alatt a Boss sebezhetetlen, majd elugrik a játékostól (`TeleportToSafeDistance()`).
- `TeleportToSafeDistance()`: Kiszámol egy random pozíciót a játékostól egy minimális távolságon kívül, és oda helyezi át a Bosst.
- `DieRoutine()` (Coroutine): Collider kikapcsolása, bónuszpontok és kill score kiosztása, Outro zene indítása, és betölti az `Outro` jelenetet.

---

## 7. Scene Scripts

### 7.1 `CutsceneManager`

**Fájl:** `Assets/Scripts/CutsceneManager.cs`

**Feladat:**

- Képsorok (Slideshow) kezelése bevezető és lezáró jelenetekhez (Intro/Outro). Panelek elhalványítása, váltása Space gombbal vagy automatikus időzítővel.

**Fő viselkedés:**

- `Start()`: Képek inicializálása, első kép aktiválása.
- `Update()`: Bemenet (Space) figyelése a manuális léptetéshez.
- `CrossfadePanel()` (Coroutine): A jelenlegi panel fokozatos eltüntetése (alpha fading) és a következő panel megjelenítése.
- A szekvencia végén a `nextScene` paraméter szerint továbbtölt. Képes megállni az utolsó képen is (`stopAtLastImage`).

### 7.2 `GateHandler`

**Fájl:** `Assets/Scripts/GateHandler.cs`

**Feladat:**

- Kapuállapot (nyitva/zárva vagy eltávolítva) megőrzése a scene-ek közötti oda-vissza mászkálás során.

**Fő viselkedés:**

- `Awake()`: Inicializálja a `gateStateKey`-t a szint-státusz menedzserből, és letiltja magát, ha a memóriában már disable-ként szerepel.
- `disableSelf()`: Elmenti a kikapcsolt állapotát, és inaktiválja a GameObjectet.

### 7.3 `ResetManager`

**Fájl:** `Assets/Scripts/ResetManager.cs`

**Feladat:**

- Globális szint-újraindító, amely a dedikált gomb (pl. R) megnyomásakor büntetéssel (HP és pontszám vesztés) újratölti az aktuális pályát.

**Fő viselkedés:**

- `ResetScene()`: Véd a duplikált hívások ellen, elindítja a `ResetRoutine`-t.
- `ResetRoutine()` (Coroutine): Képernyő elsötétítése (`ScreenFader.FadeOut`), állapotok (`JumpKingSceneTransitionState`, `SceneTransitionLevelStateManager`) resetelése, pontszámok korrekciója, és a `SceneManager.LoadScene` meghívása.

### 7.4 `SceneTeleporter`

**Fájl:** `Assets/Scripts/SceneTeleporter.cs`

**Feladat:**

- Alapvető, egyszerű trigger alapú jelenetváltó, amely rögtön átdob a beállított pályára (pl. teleport kapu).

**Fő viselkedés:**

- `OnTriggerEnter2D()`: Játékos detektálásánál végrehajtja a `SceneManager.LoadScene(sceneToLoad)` függvényt.

---

## 8. Score System Scripts

### 8.1 `ScoreManager`

**Fájl:** `Assets/Scripts/Score/ScoreManager.cs`

**Feladat:**

- A játékpontszám globális, statikus tárolója. Csomópont a különféle score források (ellenfelek, urnák) és az UI között.

**Fő viselkedés:**

- Statikus metódusok az azonnali módosításhoz (`AddPoints`, `SetScore`, `ResetScore`).
- Specifikus metódusok a könnyű hívhatóságért: `AddPatrollingEnemyKillScore()`, `AddMageKillScore()`, `AddBossKillScore()`, `AddGoldUrnScore()`.
- Eseményt (`ScoreChanged`) publikál minden változásnál az UI frissítéséhez.

### 8.2 `PlayerScoreUI`

**Fájl:** `Assets/Scripts/Score/PlayerScoreUI.cs`

**Feladat:**

- A TextMeshPro UI felület frissítése az aktuális pontszám alapján.

**Fő viselkedés:**

- `OnEnable()` / `OnDisable()`: Feliratkozás és leiratkozás a `ScoreManager.ScoreChanged` eseményére.
- `HandleScoreChanged()`: Szöveg frissítése a felületen.

### 8.3 `GoldUrnInteractable`

**Fájl:** `Assets/Scripts/Score/GoldUrnInteractable.cs`

**Feladat:**

- Pályán lévő interaktív urna. E gomb megnyomásakor pontot ad, összetörik, és elmenti az állapotát.

**Fő viselkedés:**

- `OnTriggerEnter2D()` / `OnTriggerExit2D()`: Interaktív UI panel (`ShowInteractUI`) bekapcsolása.
- `Update()`: Ha az E be van nyomva a zónában, széttöri az urnát (`BreakUrn()`).
- `BreakUrn()`: Eltünteti a tárgyat, jóváírja az 50 pontot, és elmenti az elpusztított állapotot a memóriába.

---

## 9. Scene Transition Rendszer

A rendszer a Jump-King stílusú, képernyő-széli pályaváltásokat menedzseli.

### 9.1 `SceneEdgeTransitionTrigger`

**Fájl:** `Assets/Scripts/SceneTransition/SceneEdgeTransitionTrigger.cs`

**Feladat:** Pályaváltó triggerek a határokon.
**Fő viselkedés:** `OnTriggerEnter2D` eseménynél ellenőrzi, hogy a játékos a megfelelő irányba halad-e (felfelé/lefelé), majd elmenti az átviteli sebességet és a cél EntryPoint-ot a `BeginTransition()` segítségével, végezetül betölti az új jelenetet.

### 9.2 `PlayerSceneTransitionHandler`

**Fájl:** `Assets/Scripts/SceneTransition/PlayerSceneTransitionHandler.cs`

**Feladat:** Az új pálya betöltésekor helyreteszi a játékost.
**Fő viselkedés:** A `Start()` függvényben lekérdezi, volt-e átmenet. Ha igen, megkeresi a megfelelő `SceneTransitionSpawnPoint`-ot ID alapján, oda teleportálja a játékost, és alkalmazza a megőrzött sebességet (CarryVelocity).

### 9.3 `SceneTransitionSpawnPoint`

**Fájl:** `Assets/Scripts/SceneTransition/SceneTransitionSpawnPoint.cs`

**Feladat:** Pályán belüli belépési pont definíciója egy egyedi ID-val (`entryPointId`). Erre hivatkoznak a triggerek.

### 9.4 `SceneTransitionState`

**Fájl:** `Assets/Scripts/SceneTransition/SceneTransitionState.cs`

**Feladat:** Statikus memória a töltőképernyő alatti adatok (célpont, lendület) tárolására.
**Fő viselkedés:** `BeginTransition()`, `TryConsumeTransition()`, és egy trigger-lock időzítő a dupla-váltások elkerülésére.

### 9.5 `SceneTransitionLevelStateManager`

**Fájl:** `Assets/Scripts/SceneTransition/SceneTransitionState.cs` (Közös fájlban található)

**Feladat:** Különféle pályaelemek (urnák, halott ellenségek, kapuk) perzisztenciájának biztosítása.
**Fő viselkedés:** A GameObject hierarchia útvonala alapján egyedi kulcsot (`BuildStateKey`) generál, amivel rögzíti, ha valami elpusztult vagy fel lett véve, így a pálya újbóli meglátogatásakor az elem eleve inaktívan indul (`DisableForSavedState`).

---

## 10. Puzzle és Interaktív Elemek

### 10.1 `LeverSwitch2D`

**Fájl:** `Assets/Scripts/LeverSwitch2D.cs`

**Feladat:** Interaktív kapcsoló, amely egy mozgó platformot irányít.
**Fő viselkedés:**

- Játékos trigger zónán belül E gomb lenyomására aktiválódik (`ToggleLever()`).
- Kicseréli az aktív és inaktív Sprite-ot (vizuális feedback).
- Lejátszik egy kapcsoló hangot.
- Meghívja a rákötött `MovingPlatform2D.TogglePlatform()` metódusát.

### 10.2 `PressurePlate2D`

**Fájl:** `Assets/Scripts/PressurePlate2D.cs`

**Feladat:** Súlyérzékelő nyomáskapcsoló, amely egy csúszóajtót nyit ki.
**Fő viselkedés:**

- `OnTriggerEnter2D`: Ha `Box` taggel rendelkező objektum lép rá, besüllyeszti a vizuális modellt, lejátszik egy hangot és kinyitja az ajtót (`door.OpenDoor()`). Számolja a rajta lévő dobozokat (`boxesOnPlate`), hogy több doboznál is megfelelően működjön.
- `OnTriggerExit2D`: Ha minden doboz lekerült, felemelkedik, záró hangot játszik és bezárja az ajtót (`door.CloseDoor()`).

### 10.3 `MovingPlatform2D`

**Fájl:** `Assets/Scripts/MovingPlatform2D.cs`

**Feladat:** Egyenes vonalban mozgó platform, beállított ofszet alapján.
**Fő viselkedés:**

- `Update()`-ben folyamatosan interpolál (`Vector3.Lerp`) a célpozíció felé.
- `TogglePlatform()`, `MoveUp()`, `MoveDown()` publikus metódusaival lehet vezérelni a célpontját.

### 10.4 `Door` és `Door2`

**Fájl:** `Assets/Scripts/Door.cs` és `Door2.cs`

**Feladat:** Animáció alapú ajtók nyitása és zárása (pl. rácsok).
**Fő viselkedés:**

- `Open()` / `Close()` metódusok, amelyeket gombok vagy triggerek hívnak meg.
- Triggerelik az adott Animator paramétert (`Open` / `Close`), lejátszák a hangot, és aktiválják az ajtó fizikai ütközőit (colliders).

### 10.5 `SlidingDoor`

**Fájl:** `Assets/Scripts/SlidingDoor.cs`

**Feladat:** Kódból (nem animátorból) vezérelt eltolódó ajtó.
**Fő viselkedés:** Az `openOffset` és a `speed` paraméterek alapján Lerp funkcióval nyílik vagy csukódik a `OpenDoor()` / `CloseDoor()` metódusok hatására. Hangeffektet játszik le nyitáskor.

---

## 11. Audio és Hangvezérlés

### 11.1 `MusicManager`

**Fájl:** `Assets/Scripts/MusicManager.cs`

**Feladat:** A megfelelő háttérzene automatikus lejátszása a különböző pályákon.
**Fő viselkedés:**

- Singleton és `DontDestroyOnLoad`, hogy a zene megszakítás nélkül folytatódhasson a jelenetváltások alatt.
- `OnSceneLoaded` eseményre feliratkozva kiolvassa a `levelMusicMap` struct tömbből, hogy az adott nevű jelenethez melyik zene tartozik, és lecseréli az AudioSource klipjét.

### 11.2 `SoundMixerManager`

**Fájl:** `Assets/Scripts/SoundMixerManager.cs`

**Feladat:** Globális hangerő-szabályozás, az Unity AudioMixer kezelése.
**Fő viselkedés:**

- Singleton, perzisztens objektum.
- Betölti a `PlayerPrefs`-ből az elmentett értékeket.
- `SetMasterVolume`, `SetMusicVolume`, `SetSFXVolume` metódusaival logaritmikus skálán állítja be a mixert ( `Mathf.Log10(volume) * 20f` ).

---

## 12. Menü, UI és Vizuális Effektek

### 12.1 `MainMenu`

**Fájl:** `Assets/Scripts/MainMenu.cs`

**Feladat:** A játék indítása, kilépés és a toplista betöltése.
**Fő viselkedés:**

- Játék indításakor nullázza a visszamaradt játékos HP-t (`PlayerHealthManager.ClearPersistedHealth()`).
- Támogatja a JSON formátumban kimentett Highscore (Toplista) betöltését és dinamikus UI generálását egy Leaderboard panelre.

### 12.2 `PauseMenuHandler`

**Fájl:** `Assets/Scripts/PauseMenuHandler.cs`

**Feladat:** Szünet menü kezelése a játék közben.
**Fő viselkedés:**

- `Update()`-ben figyeli az Escape gombot.
- Megnyitáskor megállítja az időt (`Time.timeScale = 0f`), és megjeleníti az UI panelt.
- Közvetlenül összeköti az UI Slidereket (Hangerő csúszkák) a `SoundMixerManager` funkcióival.

### 12.3 `GameOverHandler`

**Fájl:** `Assets/Scripts/GameOverHandler.cs`

**Feladat:** Halál képernyőn lévő opciók.
**Fő viselkedés:** Lehetővé teszi az újrakezdést a pályán (`PlayerHealthManager.ResetHealth()` meghívásával) vagy a kilépést.

### 12.4 `GameWinHandler`

**Fájl:** `Assets/Scripts/GameWinHandler.cs`

**Feladat:** A játék megnyerésének menedzselése.
**Fő viselkedés:**

- Elkéri a játékos nevét (InputField) és a végső pontszámot (`ScoreManager.CurrentScore`).
- JSON struktúrába formázza, rendezi, és elmenti a legjobb 10 játékos közé a `PlayerPrefs` "Leaderboard" kulcs alá.

### 12.5 `SettingsMenuUI`

**Fájl:** `Assets/Scripts/SettingsMenuUI.cs`

**Feladat:** Tisztán UI szkript a beállítások menüben található csúszkák feltöltésére és bekötésére a `SoundMixerManager`-be.

### 12.6 `ScreenFader`

**Fájl:** `Assets/Scripts/ScreenFader.cs`

**Feladat:** Sötétítő átmenetek rajzolása (Fade In / Fade Out) jelenetek és teleportációk között.
**Fő viselkedés:**

- UI Image Alpha értékének folyamatos módosítása Coroutine segítségével.
- Statikus segédfüggvényei (`FadeOut`, `FadeIn`, `FadeScreen`) globálisan elérhetőek minden más szkript számára.

### 12.7 `ShowInteractUI`

**Fájl:** `Assets/Scripts/ShowInteractUI.cs`

**Feladat:** Értesíti a játékost, hogy egy objektum interaktálható.
**Fő viselkedés:** Trigger zónába érve aktivál egy UI GameObject-et (pl. lebegő E-betűt vagy magyarázó szöveget), és kikapcsolja azt távozáskor.

### 12.8 `UniversalPulsing`

**Fájl:** `Assets/Scripts/UniversalPulsing.cs`

**Feladat:** Vizuális pulzáló effekt UI vagy 2D Sprite elemeken.
**Fő viselkedés:** Egy Coroutine segítségével `Mathf.MoveTowards` metódussal változtatja a tárgy `transform.localScale`-jét egy megnövelt és lecsökkentett határ (`growthBound`, `shrinkBound`) között.

### 12.9 `EffectSpawner`

**Fájl:** `Assets/Scripts/EffectSpawner.cs`

**Feladat:** Periodikus vizuális effekt (pl. varázslat kör) lerakása a világba.
**Fő viselkedés:** `InvokeRepeating` segítségével adott időközönként példányosít egy Prefabot a `transform.position + offset` helyre.

---

## 13. Egyéb és Veszélyforrások

### 13.1 `SpikeHitbox`

**Fájl:** `Assets/Scripts/SpikeHitbox.cs`

**Feladat:** Passzív sebző terepelemek (tüskék).
**Fő viselkedés:** `OnCollisionEnter2D` és `OnCollisionStay2D` eseményeknél detektálja a játékost, és folyamatos vagy egyszeri sebzést küld a `PlayerHealthManager.TakeDamage` metódusnak.

### 13.2 `SimpleBullet`

**Fájl:** `Assets/Scripts/SimpleBullet.cs`

**Feladat:** Alapvető lövedék, melyet a Boss vagy egyéb lőfegyverek használnak.
**Fő viselkedés:** Beállított sebességgel (`speed`) halad előre a saját lokális tengelyén (`transform.right`). Ha falnak vagy akadálynak ütközik, azonnal megsemmisül. Ütközéskor sebzi a Player-t.

### 13.3 `CollisionDetector`

**Fájl:** `Assets/Scripts/CollisionDetector.cs`

**Feladat:** Általános ütközésvizsgáló, amely `UnityEvent` eseményeket hív meg külső logikák számára.
**Fő viselkedés:** Az `OnCollisionEnter2D` / `Exit2D` detektálásánál megnézi, hogy a beállított szkript (`_colliderScript`) rajta van-e az objektumon, és ha igen, elsüti a publikus eventet.

### 13.4 `SelfDestruct`

**Fájl:** `Assets/Scripts/SelfDestruct.cs`

**Feladat:** Automatikus szemétgyűjtő szkript partiklikhoz vagy átmeneti lövedékekhez.
**Fő viselkedés:** A beállított `destroyDelay` után törli magát a jelenetből a memóriaszivárgások elkerülésére.

---

## 14. Automata Tesztelés (Unit Tests)

A projekt kiemelkedő jellemzője a kiterjedt Unit Test lefedettség, amely biztosítja a kód stabilitását a módosítások során. A tesztek az NUnit / Unity Test Framework használatával íródtak az `Assets/Tests` könyvtárban.

### 14.1 `TestUtilities`

Segédosztály a tesztek számára. Reflection (C#) használatával hozzáfér privát mezőkhöz és metódusokhoz, és reseteli a statikus memóriákat (pl. `JumpKingSceneTransitionState`, `ScoreManager`) az egyes tesztek között.

### 14.2 Tesztelt Területek

- **`PlayerTests`**: Ellenőrzi a játékos fizikai ugrását, a charge jump magasságának skálázódását, és az Animator változók frissülését.
- **`EnemyTests` (Mage, Patrolling)**: Validálja az ellenségek járőrvonalát, trigger reakcióidejét, a stomp mechanikák élet-csökkentő hatásait, és a Mágus teleport / sebezhetetlen ablakainak stabilitását.
- **`SceneTransitionTests`**: Biztosítja az `EntryPointID`-k épségét, a `LevelStateManager` statikus gyűjteményeinek helyes hozzáadását/lekérdezését (pl. felvett arany), és a Trigger-lock rendszert.
- **`Misc & UI Tests`**: Lefedi a `CutsceneManager` átmeneteit, a kapuk állapotmentését, a toplista JSON betöltésének helyességét, és az overflow védelmeket a pontszámlálóban.
