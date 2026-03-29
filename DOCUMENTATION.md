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
