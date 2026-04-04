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
