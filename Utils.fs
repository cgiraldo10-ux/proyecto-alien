module App.Utils

open System

// Funcion mejorada con proteccion de limites de pantalla
let mostrarMensaje x y color (msg:string) =
    // Solo dibuja si las coordenadas estan dentro de la pantalla actual
    if x >= 0 && x < Console.BufferWidth && y >= 0 && y < Console.BufferHeight then
        try
            Console.SetCursorPosition(x, y)
            Console.ForegroundColor <- color
            msg |> Console.Write
        with _ -> () // Evita que cualquier imprevisto cierre el juego