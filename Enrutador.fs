module App.Enrutador

open App.Tipos

let rec loopPrincipal estado =
    match estado with 
    | MostrarMenu ->
        match App.Menu.mostrar() with 
        | NuevoJuego -> 
            App.Juego.mostrar false
            loopPrincipal MostrarMenu
        | CargarPartida -> 
            App.Juego.mostrar true
            loopPrincipal MostrarMenu
        | Salir -> ()
    | Terminar -> ()

let mostrar() = loopPrincipal MostrarMenu