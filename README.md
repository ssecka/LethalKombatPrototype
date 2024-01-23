# LethalKombatPrototype
2.5D Fighting game

Repo: https://github.com/ssecka/LethalKombatPrototype
Video: https://www.youtube.com/watch?v=g2JY8QZeDH4


### Gruppenmitglieder:
- Salif Secka 208959
- Telmo Santos Carneiro 208947
- Stefan Kurz 208904


### How to Start Playing
Die erste Instanz kann in Unity einfach durch Drücken von Play gestartet werden.
- Wenn Sie eine zweite Instanz auf Ihrem Computer starten möchten, können Sie dies ebenfalls tun.
    - Dazu müssen Sie das Projekt builden und die daraus resultierende .exe-Datei ausführen.

### Beschreibung
Bei Lethal Kombat handelt es sich um ein Player versus Player Fighter, indem 2 Spieler gegeneinander antreten. Lethal Kombat ist von der Street Fighter Series inspiriert. Es sind folgende Aktionen verfügbar:

- Movement (wsad)
- Jump (Space)
- Jab (Left mouse button)
- Hook (L)
- Sidekick (Right mouse Button)
- Fastkick (J)
- Haduken / Fireball (K)
- Block (SHIFT)

![ControllerLayout](ControllerSVG.png)

### Herausforderungen
- Die Umstellung von Local Multiplayer auf Online Multiplayer mit Fusion war der schwierigste Teil unseres Projekts, da die Dokumentation nicht sehr hilfreich war und wir sehr wenig Erfahrung mit Fusion hatten.
- Es war schwierig, Server und Client zu synchronisieren.  
    - Anfangs wurde die UI nur beim Host aktualisiert.
    - Die Animationen und auch die VFX wurden nur beim Host abgespielt.

### Lessons Learned
- Eigene Animationen mit [Cascadeur](https://cascadeur.com/) erstellen
- Networking Synchronisierung

### Inspirationen
 - [Mortal Kombat](https://www.youtube.com/watch?v=0HEE78L_CnA)
 - [Street Figter](https://youtu.be/Tb521YYYkaE?si=BafHJbBSefCwxCjg&t=44)    

### Prefabs
- [HealthSystem](https://assetstore.unity.com/packages/tools/utilities/health-system-for-dummies-215755#description)

### Benutzte Dateien / Tutorials:

Assets/Scripts/Animations/Tick_Accurate/AnimatorStateSync.cs: 
- größtenteils aus dem Tutorial: https://doc.photonengine.com/fusion/current/technical-samples/animations#example-3---state-synchronization-with-animator | Example 3
übernommen

Assets/Scripts/Animations/Tick_Accurate/Player_AnimatorStateSync.cs: 
- stammt ebenfalls vom oben genannten Tutorial, wurde jedoch für unsere Projekt stark abgeändert.

https://www.youtube.com/watch?v=gxhG9BHcxmc&list=PLyDa4NP_nvPfHhPuumJylSj8jXyULsT1X&index=4
- Funktionsweise von Networked HP changes 

https://www.youtube.com/watch?v=Dn6pzf8scco&list=PLyDa4NP_nvPfHhPuumJylSj8jXyULsT1X&index=7
- Im Tutorial wird gezeigt wie eine Rackete für Fusion erzeugt wird im Bereich eines First Person Shooters. Wir haben die Idee dahinter auf den Fireball angewendet

Mixamo Animationen:
- Knockout
- Walking Animations
- Fireball

Sounds
- Hadouken Sound von Street Fighter
- Jab / Punch / SideKick / Kick von Diablo 2
