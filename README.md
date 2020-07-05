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
Es posible cambiar el BPM para hacer el beat mas rapido o lento (hay un pequeño delay mientras se ajusta el sonido despues de cambiar el BPM en runtime)

#### Unas notas finales: 

Es la primera vez que uso ***OnAudioFilterRead*** cosas que podrian mejorar: 

- mejorar el sonido, ahora hay una distorsion. Creo que algo pasa con el samplerate o la configuracion de los clips importados que esta generando eso. 
- el double kick tiene un espacio entre los kick fijos, creo que esto se podria calcular de acuerdo al BPM actual. 
- en general no estoy seguro de si esta es la mejor implementacion de un drum kit procedural. 

Los assets de artes fuerón proveídos por: [Kenney](https://www.kenney.nl/assets/scribble-platformer)
