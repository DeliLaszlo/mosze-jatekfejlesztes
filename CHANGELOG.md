# Changelog

A projekt fontosabb változásai itt kerülnek rögzítésre.

## [2026-05-10]

### Added

- Linux build profile hozzáadva

### Changed

- `GameOverHandler` reset bug javítja
- `PlayerHealthManager` invincibility frame és realisztikusabb death animáció ugrás közben
- `boss_shoot` hard crash javítva a boss legyőzés után

## [2026-04-29]

### Added

- **Teljes automata tesztelési keretrendszer (Unit Tests)** hozzáadva a projekthez (`Assets/Tests` mappa).
- `TestUtilities`: Privát mezők és metódusok elérését segítő utility osztály (Reflection alapokon).
- Játékos logika tesztek: `PlayerTests` (mozgás, töltés, input, ugrás, sebződés).
- Ellenfél tesztek: `PatrollingEnemyTests` és `MageEnemyTests` (állapotgép, sebződés, támadás, stomp).
- Rendszer tesztek: `SceneTransitionTests` (átmenetek, spawn pontok), `ScoreManagerTests`, `GoldUrnInteractableTests`, `PlayerScoreUITests`.
- Egyéb tesztek: `MiscTests` (kisebb util osztályok, kapukezelés).

## [2026-04-19]

### Added

- Új jelenetek létrehozva: `Intro`, `EnemyLevel`, `Outro`.
- Új `CutsceneManager` script: panelalapú intro képsorok kezelése, fade váltás, továbblépés a következő jelenetre.
- Új `GateHandler` script: kapu letiltási állapot megőrzése scene váltások között.
- Új score rendszer alap script: `ScoreManager` (globális pontszám és eseménykezelés).
- Új UI script: `PlayerScoreUI` (pontszám kijelzés TMP szövegmezőn).
- Új interakciós script: `GoldUrnInteractable` (E gombos urna törés, pont jóváírás, állapotmentés).
- Új scene váltás kezelő scriptek: `PlayerSceneTransitionHandler`, `SceneEdgeTransitionTrigger`, `SceneTransitionSpawnPoint`, `SceneTransitionState`

## [2026-04-04]

### Added

- Új `MageEnemy` viselkedés: állapotgép alapú ellenfél (`Shielded`, `Attacking`, `Vulnerable`, `Teleporting`, `Dead`) időzített támadással, sebezhető ablakkal, teleportálással és halál állapottal.
- Új slam támadás logika: animáció eseményhez kötött területi sebzés (`OverlapBoxAll`) a `PlayerHealthManager` felé.
- Új stomp logika: sebezhető állapotban HP csökkentés/halál, nem sebezhető állapotban játékos büntetése sebzéssel és teleporttal.
- Új stomp trigger továbbító script: `MageStompReciever`, amely a játékos trigger belépését a `MageEnemyController` felé továbbítja.

## [2026-03-29]

### Added

- Új `PatrollingEnemy` viselkedés: két pont közötti járőrözés, játékos észlelés trigger alapján, támadás animációval, valamint stomp-halál kezelés.

## [2026-03-22]

### Added

- Új projekt specifikáció: `specification.md` (Aetheria teljes szoftverfejlesztési specifikáció).
- Új dokumentációs fájlok létrehozva: `DOCUMENTATION.md`, `Sprint1.md`.
- README bővítve játékáttekintéssel, fő játékelemekkel, irányítás táblázattal és tisztább futtatási útmutatóval.

### Changed

- `.gitignore` frissítve: `.vscode/` cache mappa kizárása.
- `.gitignore` frissítve: `*.slnx` fájlok kizárása a verziókezelésből.

## [2026-03-15]

### Added

- Új `PlayerController` rendszer: oldalirányú mozgás, töltésalapú ugrás, talajérzékelés, animátor paraméterezés és procedurális charge-pose vizuális visszajelzés.
