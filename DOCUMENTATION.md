# Aetheria dokumentáció

Ez a fájl az Aetheria játék részletes dokumentációját tartalmazza.

## 1. Player Scripts

### 1.1 `PlayerController`

**Fájl:** `Assets/Player/Scripts/PlayerController.cs`

**Feladat:**

- Játékosmozgás (bal/jobb), tölthető ugrás, földkapcsolat ellenőrzése.
- Animator paraméterek frissítése és állapotok kényszerített szinkronja.
- Vizuális charge pose (squash/stretch, tint).

**Fő viselkedés:**

- `Update()`: input olvasás, charge kezelés, ugrás indítás, animator frissítés.
- `FixedUpdate()`: fizikában mozgás alkalmazása, grounded állapot számítás.
- `ApplyFacing()`: karakter irányba forgatása.
- `UpdateChargePose()`: töltés közbeni vizuális torzítás.

### 1.2 `PlayerHealthManager`

**Fájl:** `Assets/Player/Scripts/PlayerHealthManager.cs`

**Feladat:**

- Életpontok nyilvántartása, sebzés fogadása, halál állapot kezelése.
- Életváltozás esemény (`HealthChanged`) publikálása UI felé.

**Fő viselkedés:**

- `Awake()`: max/current health inicializálás.
- `TakeDamage()`: HP csökkentés, sebzés animáció, halál ellenőrzés.
- `HandleDeath()`: animáció, irányítás/collider/fizika tiltása.

### 1.3 `PlayerHealthBlocksUI`

**Fájl:** `Assets/Player/Scripts/PlayerHealthBlocksUI.cs`

**Feladat:**

- HP blokkos UI építés és frissítés a `PlayerHealthManager` alapján.
- Színváltás életszinttől függően.

**Fő viselkedés:**

- `BuildBlocks()`: UI elemek dinamikus létrehozása.
- `RefreshBlocks()`: kitöltött/üres blokkok és aktív szín frissítése.
- `EnsureHorizontalLayout()`: elrendezés automatikus beállítása.

## 2. Patrolling Enemy Scripts

### 2.1 `PatrollingEnemy`

**Fájl:** `Assets/PatrollingEnemy/Scripts/PatrollingEnemy.cs`

**Feladat:**

- Két pont (`pointA`, `pointB`) között folyamatos járőr mozgás.
- Irányváltáshoz sprite orientáció frissítése.
- Támadási logika a játékos közelében.
- Stomp esetén halál állapot kezelése.

**Fő viselkedés:**

- `Start()`: komponensek keresése, patrol határok inicializálása.
- `FixedUpdate()`: vízszintes mozgás, határoknál irányváltás.
- `HandleAttackRangeTrigger()`: játékos felderítése, támadás indítása.
- `AttackRoutine()`: támadás animáció, sebzés küldése (`PlayerHealthManager.TakeDamage`).
- `DieFromStomp()`: halál animáció, collider/physics kikapcsolása.

**Fontos Inspector mezők:**

- Patrol: `pointA`, `pointB`, `moveSpeed`
- Attack: `attackTriggerName`, `attackDuration`, `destroyPlayerOnAttack`
- Death: `deathTriggerName`

### 2.2 `EnemyAttackRange`

**Fájl:** `Assets/PatrollingEnemy/Scripts/AttackHitboxHandler.cs`

**Feladat:**

- Trigger zónában követi a játékost, és periodikusan újrajelenti az ellenfél tulajdonosnak (`PatrollingEnemy`).

**Fő viselkedés:**

- `OnTriggerEnter2D()`: játékos felvétele követésbe, támadásjelzés.
- `OnTriggerExit2D()`: játékos eltávolítása követésből.
- `MonitorPlayersInRange()`: periodikus ellenőrzés, hogy a triggerben maradt-e célpont.

### 2.3 `Stomp`

**Fájl:** `Assets/PatrollingEnemy/Scripts/DeathByPlayer.cs`

**Feladat:**

- Stomp triggeren keresztül az ellenfél megsemmisítése vagy halál logika meghívása.

**Fő viselkedés:**

- `OnTriggerEnter2D()`: ha a játékos lép be, meghívja az ellenfél `DieFromStomp()` metódusát.

## 3. Mage Enemy Scripts

### 3.1 `MageEnemyController`

**Fájl:** `Assets/MageEnemy/Scripts/MageEnemyController.cs`

**Feladat:**

- Állapotgép alapú mage ellenfél vezérlése (`Shielded`, `Attacking`, `Vulnerable`, `Teleporting`, `Dead`).
- Időzített támadásciklus kezelése, majd sebezhető ablak megnyitása.
- Slam támadásból területi sebzés alkalmazása a játékosra.
- Teleportálás több pontra, vizuális effektekkel és szín-visszajelzéssel.
- Stomp esemény kezelése: sebezhető állapotban mage sebzése, egyébként játékos büntetése.

**Fő viselkedés:**

- `Start()`: komponensek és referencia mezők inicializálása, HP beállítás, teleport pont kezdő index meghatározás, `Shielded` állapot indítása.
- `Update()`: állapotonkénti időzítés és átmenet (`Shielded` -> támadás, `Vulnerable` -> teleport).
- `StartAttack()`: támadás állapotba lépés és animáció trigger (`Attack`).
- `OnStaffHitGround()`: animáció eseményből slam VFX lejátszás és sebzés kiosztása.
- `OnAttackFinished()`: támadás után sebezhető állapotba váltás.
- `HandleTeleportSequence()`: eltűnés/megjelenés VFX, sprite/collider ideiglenes tiltás, pozícióváltás, visszatérés `Shielded` állapotba.
- `ApplySlamDamage()`: `OverlapBoxAll` alapú találatvizsgálat a slam hitbox területén, játékos sebzése.
- `HandleStomp()`: sebezhető állapotban mage HP csökkentése vagy halál; máskülönben büntető teleport rutin indítása.
- `PunishPlayerSequence()`: játékos sebzése, majd teleportálása egy másik pontra.
- `Die()`: halál állapot, animáció trigger, collider és fizika kikapcsolás.

**Fontos Inspector mezők:**

- Combat: `maxHealth`, `timeBetweenAttacks`, `vulnerableDuration`, `slamHitbox`
- Teleport: `teleportPoints`, `rootTransform`, `damageTeleportOutColor`
- VFX: `shieldParticles`, `slamParticles`, `teleportOutParticles`, `teleportInParticles`
- Render/animáció: `spriteRenderer`

### 3.2 `MageStompReciever`

**Fájl:** `Assets/MageEnemy/Scripts/MageStompReciever.cs`

**Feladat:**

- Stomp triggeren beérkező játékos találat továbbítása a mage vezérlő script felé.

**Fő viselkedés:**

- `OnTriggerEnter2D()`: ha a belépő collider `Player` tagű, meghívja a `mageController.HandleStomp(...)` metódust.

## 4. Scene Scripts

### 4.1 `CutsceneManager`

**Fájl:** `Assets/Scripts/CutsceneManager.cs`

**Feladat:**

- Bevezető képsor (`Intro`) paneljeinek időzített kezelése.
- Panelváltás fade animációval.
- Továbbhaladás a következő jelenetre (`nextScene`) automatikusan vagy Space gombbal.

**Fő viselkedés:**

- `Start()`: panelek inicializálása, első panel aktiválása, időzítő indítása.
- `Update()`: Space gomb figyelése és manuális továbbléptetés.
- `AutoSwitch()`: adott idő után váltás.
- `CrossfadePanel()`: panel fade-out/fade-in animáció.

### 4.2 `GateHandler`

**Fájl:** `Assets/Scripts/GateHandler.cs`

**Feladat:**

- Kapu állapotának (aktív/inaktív) megőrzése scene átmenetek között.
- Mentett állapot visszaállítása betöltéskor.

**Fő viselkedés:**

- `Awake()`: egyedi gate key építése és mentett állapot alkalmazása.
- `disableSelf()`: kapu letiltása és állapot mentése (`SceneTransitionLevelStateManager`).

## 5. Score System Scripts

### 5.1 `ScoreManager`

**Fájl:** `Assets/Scripts/Score/ScoreManager.cs`

**Feladat:**

- Játékpontszám központi, statikus kezelése.
- Pontok hozzáadása külön eseménytípusok alapján.
- Score változás esemény (`ScoreChanged`) publikálása UI komponenseknek.

**Fő viselkedés:**

- `ResetScore()`: score nullázása.
- `AddPoints()`: pont hozzáadása overflow védelemmel.
- `AddPatrollingEnemyKillScore()`: +200 pont.
- `AddMageKillScore()`: +1000 pont.
- `AddGoldUrnScore()`: +50 pont.

### 5.2 `PlayerScoreUI`

**Fájl:** `Assets/Scripts/Score/PlayerScoreUI.cs`

**Feladat:**

- A jelenlegi score megjelenítése TMP UI elemen.
- Feliratkozás a `ScoreManager.ScoreChanged` eseményre.

**Fő viselkedés:**

- `OnEnable()`: esemény feliratkozás és azonnali UI frissítés.
- `OnDisable()`: esemény leiratkozás.
- `HandleScoreChanged()`: szöveg frissítése az aktuális pontszámra.

### 5.3 `GoldUrnInteractable`

**Fájl:** `Assets/Scripts/Score/GoldUrnInteractable.cs`

**Feladat:**

- Interaktív urna kezelése trigger zónában.
- E gombos interakció érzékelése (Input System + fallback).
- Urna törésének és pontjutalomnak mentése scene átmenetek között.

**Fő viselkedés:**

- `OnTriggerEnter2D()` / `OnTriggerExit2D()`: játékos közelség követése, interakciós UI kapcsolása.
- `Update()`: interakció figyelése, törés indítása.
- `BreakUrn()`: állapot mentése, +50 pont jóváírása, objektum letiltása.

## 6. Scene struktúra

### 6.1 `Intro`

- Bevezető jelenet, ahol a `CutsceneManager` képsorokat vált és a játékost továbbítja a következő pályára.

### 6.2 `EnemyLevel`

- Fő játékmenet jelenet, ahol a score rendszer (`ScoreManager`, `PlayerScoreUI`, `GoldUrnInteractable`) és a kapuállapot-kezelés (`GateHandler`) aktív.

### 6.3 `Outro`

- Lezáró jelenet a pálya/célállapot elérése után.

## 7. Scene Transition rendszer

### 7.1 `SceneEdgeTransitionTrigger`

**Fájl:** `Assets/Scripts/SceneTransition/SceneEdgeTransitionTrigger.cs`

**Feladat:**

- Trigger belépéskor scene váltás indítása (`targetSceneName`).
- Cél belépési pont (`targetEntryPointId`) és továbbvitt sebesség megadása.
- Irány alapú feltételkezelés (csak felfelé vagy lefelé mozgásnál váltson).

**Fő viselkedés:**

- `OnTriggerEnter2D()`: játékos detektálása, feltételek ellenőrzése, átmenet indítása.
- `JumpKingSceneTransitionState.BeginTransition()`: cél entry point + carry velocity mentése.
- `JumpKingSceneTransitionState.LockTriggers()`: duplaváltás elleni zárolás.
- `SceneManager.LoadScene()`: cél scene betöltése.

### 7.2 `PlayerSceneTransitionHandler`

**Fájl:** `Assets/Scripts/SceneTransition/PlayerSceneTransitionHandler.cs`

**Feladat:**

- Új scene betöltése után a játékos pozicionálása a megfelelő `SceneTransitionSpawnPoint`-ra.
- Opcionális sebességátvitel (`carryVelocity`) alkalmazása az új scene-ben.

**Fő viselkedés:**

- `Start()`: függőben lévő átmenet fogyasztása (`TryConsumeTransition`).
- Spawn pont keresése `entryPointId` alapján.
- Játékos pozíció és (opcionálisan) `Rigidbody2D.linearVelocity` beállítása.
- Utólagos trigger lock (`postSpawnTriggerLockDuration`) alkalmazása.

### 7.3 `SceneTransitionSpawnPoint`

**Fájl:** `Assets/Scripts/SceneTransition/SceneTransitionSpawnPoint.cs`

**Feladat:**

- Belépési pont azonosítók (`entryPointId`) definiálása az egyes jelenetekben.

**Fő viselkedés:**

- `EntryPointId`: üres értéknél a `Default` azonosítót adja vissza.
- `OnValidate()`: editorban biztosítja, hogy ne maradjon üres azonosító.

### 7.4 `SceneTransitionState`

**Fájl:** `Assets/Scripts/SceneTransition/SceneTransitionState.cs`

**Feladat:**

- Statikus átmeneti állapot tárolása scene betöltések között.
- Trigger lock időablak kezelése az ismételt aktiválások ellen.

**Fő viselkedés:**

- `BeginTransition()`: entry point + sebesség mentése.
- `TryConsumeTransition()`: egyszer használatosan visszaadja a mentett átmenetet.
- `LockTriggers()`: minimális várakozás beállítása következő triggerig.

### 7.5 `SceneTransitionLevelStateManager`

**Fájl:** `Assets/Scripts/SceneTransition/SceneTransitionState.cs`

**Feladat:**

- Scene-ek között perzisztens pályaállapot kezelése (urna, ellenfél, kapu).
- Stabil kulcsképzés (`BuildStateKey`) scene útvonal + hierarchia alapján.

**Fő viselkedés:**

- `MarkUrnBroken()` / `IsUrnBroken()`
- `MarkEnemyDefeated()` / `IsEnemyDefeated()`
- `MarkGateDisabled()` / `IsGateDisabled()`
- `DisableForSavedState()`: mentett állapot szerinti objektum letiltás.

### 7.6 `SceneTeleporter`

**Fájl:** `Assets/Scripts/SceneTeleporter.cs`

**Feladat:**

- Egyszerű, trigger alapú scene váltás biztosítása.

**Fő viselkedés:**

- `OnTriggerEnter2D()`: ha a Player belép, a `sceneToLoad` jelenetre vált.
