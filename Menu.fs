module App.Menu

open System
open App.Utils
open App.Tipos

let mostrar () =
    Console.Clear()
    Console.CursorVisible <- false
    let opciones = [| "Nueva Partida"; "Cargar Partida"; "Salir" |]
    let mutable seleccionada = 0
    let mutable salirMenu = false

    while not salirMenu do
     
        mostrarMensaje 10 2 ConsoleColor.Magenta "//////////////////////////////////"
        mostrarMensaje 11 3 ConsoleColor.Cyan    " ALIEN VERSUS MONSTRUO MALO PIU  "
        mostrarMensaje 10 4 ConsoleColor.Magenta "/////////////////////////////////"

        
        for i in 0 .. opciones.Length - 1 do
            if i = seleccionada then
                mostrarMensaje 15 (7 + i * 2) ConsoleColor.Green (sprintf "-> [ %s ]" opciones.[i])
            else
                mostrarMensaje 15 (7 + i * 2) ConsoleColor.Gray  (sprintf "   %s   " opciones.[i])

        mostrarMensaje 10 15 ConsoleColor.DarkGray "Usa flechitas para moverte y Enter para seleccionar"

        let tecla = Console.ReadKey(true)
        match tecla.Key with
        | ConsoleKey.UpArrow -> 
            seleccionada <- if seleccionada > 0 then seleccionada - 1 else opciones.Length - 1
        | ConsoleKey.DownArrow -> 
            seleccionada <- if seleccionada < opciones.Length - 1 then seleccionada + 1 else 0
        | ConsoleKey.Enter -> 
            salirMenu <- true
        | _ -> ()
    
    match seleccionada with
    | 0 -> NuevoJuego
    | 1 -> CargarPartida
    | _ -> Salir