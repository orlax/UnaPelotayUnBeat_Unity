# UnaPelotayUnBeat_Unity
Un proyecto de unity con dos escenas, uno es un sistema de fisicas pequeñito en el que rebotar una pelota, y otro es una escena con un beat generado proceduralmente. 

## Escena: UnaPeloaAnimada

![Imagen de una pelota 2d rebotando](https://media.giphy.com/media/LNwdFl77Kbu30q3bSv/giphy.gif)

Esta escena tiene una pelota amarilla, que puede ser clickeada, arrastrada y soltada para rebotar. en su jerarquia los objetos son: 

- BALL, la pelota con el script de *OrlaPelota* que implementa la interface *OrlaFisicasinterface* 
- CURSOR, imagen del cursor con el script *CursorController* que cambia la pocision del mouse, y permite clickear la pelota
- OrlaFisicas, objeto con el script *OrlaFisicas* que implementa toda la funcionalidad del sistema de fisicas, colisiones y demas. Tambien tiene *OrlaSound*
una implementacion super sencilla de un sistema de audio. 

## Escena: UnaBeatChevere 

Esta escena tiene un solo objeto importante, ** Beat ** que tiene el script *OrlaBeat*, este script tiene un KickClip y un SnareClip que se utilizan para producir un beat proceduralmente. 

Los assets de artes fuerón proveídos por: [Kenney](https://www.kenney.nl/assets/scribble-platformer)
