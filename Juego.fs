module App.Juego

open System
open System.IO
open System.Threading
open App.Utils
open App.Tipos

let RutaGuardado = "savegame.txt"

let guardarPartida state =
    try
        let datos = sprintf "%d,%d" state.Puntos state.Vidas
        File.WriteAllText(RutaGuardado, datos)
    with _ -> ()

let cargarPartida () =
    if File.Exists(RutaGuardado) then
        try
            let contenido = File.ReadAllText(RutaGuardado)
            let partes = contenido.Split(',')
            (int partes.[0], int partes.[1])
        with _ -> (0, 3)
    else (0, 3)

let estadoInicial () = {
    ProgramState = Running
    AlienX = Console.BufferWidth / 2
    AlienY = Console.BufferHeight / 2
    AlienEstado = Vivo
    ColisionAlien = 0
    RedibujarPantalla = true
    Tick = -1
    Misiles = []
    EnemigoX = Console.BufferWidth - 2
    EnemigoY = 0
    EnemigoDir = 1
    EnemigoEstado = Vivo
    ColisionEnemigo = 0
    MisilesEnemigos = []
    Puntos = 0
    Vidas = 3
}

let actualizarTick state = { state with Tick = state.Tick + 1 }

let actualizarMisiles state =
    if state.Misiles <> [] then 
        state.Misiles
        |> Seq.map (fun misil -> { misil with X = misil.X + 1 })
        |> Seq.filter (fun misil -> misil.X < Console.BufferWidth - 2)
        |> Seq.toList
        |> fun nuevosMisiles -> { state with Misiles = nuevosMisiles; RedibujarPantalla = true }
    else state

let actualizarMisilesEnemigos state =
    if state.MisilesEnemigos <> [] then 
        state.MisilesEnemigos
        |> Seq.map (fun misil -> { misil with X = misil.X - 1 })
        |> Seq.filter (fun misil -> misil.X >= 0)
        |> Seq.toList
        |> fun nuevosMisiles -> { state with MisilesEnemigos = nuevosMisiles; RedibujarPantalla = true }
    else state

let actualizarEnemigo state =
    if state.EnemigoEstado = Vivo && state.Tick % 4 = 0 then 
        let nuevoY = state.EnemigoY + state.EnemigoDir
        let nuevaDir, Y = 
            match nuevoY with 
            | y when y > Console.BufferHeight - 1 -> -1, Console.BufferHeight - 1
            | y when y < 0 -> 1, 0
            | _ -> state.EnemigoDir, nuevoY
        { state with EnemigoY = Y; EnemigoDir = nuevaDir; RedibujarPantalla = true }
    else state

let dispararMisilesEnemigos state =
    if state.EnemigoEstado = Vivo && state.Tick % 10 = 0 then 
        let nuevoMisil = { X = state.EnemigoX - 2; Y = state.EnemigoY }
        { state with MisilesEnemigos = nuevoMisil :: state.MisilesEnemigos; RedibujarPantalla = true }
    else state

let detectarColisionAlien state =
    if state.AlienEstado = Muerto then state
    else
        state.MisilesEnemigos
        |> List.filter (fun misil -> not (misil.Y = state.AlienY && misil.X = state.AlienX + 1))
        |> fun nuevosMisiles ->
            if nuevosMisiles.Length <> state.MisilesEnemigos.Length then
                let nuevasVidas = state.Vidas - 1
                let estadoPrograma = if nuevasVidas <= 0 then GameOver else Running
                { state with 
                    AlienEstado = Muerto
                    MisilesEnemigos = nuevosMisiles
                    RedibujarPantalla = true
                    ColisionAlien = state.Tick
                    Vidas = nuevasVidas
                    ProgramState = estadoPrograma }
            else state 

let detectarColisionEnemigo state =
    if state.EnemigoEstado = Muerto then state
    else
        state.Misiles
        |> List.filter (fun misil -> not (misil.Y = state.EnemigoY && misil.X = state.EnemigoX - 1))
        |> fun nuevosMisiles ->
            if nuevosMisiles.Length <> state.Misiles.Length then
                let nuevosPuntos = state.Puntos + 100
                let tState = { state with 
                                EnemigoEstado = Muerto
                                Misiles = nuevosMisiles
                                RedibujarPantalla = true
                                ColisionEnemigo = state.Tick
                                Puntos = nuevosPuntos }
                guardarPartida tState
                tState
            else state 

let resetAlien state =
    if state.AlienEstado = Muerto && state.ProgramState = Running then 
        let tiempo = state.Tick - state.ColisionAlien
        if tiempo >= 40 then { state with AlienEstado = Vivo; RedibujarPantalla = true }
        else state
    else state

let resetEnemigo state =
    if state.EnemigoEstado = Muerto then 
        let tiempo = state.Tick - state.ColisionEnemigo
        if tiempo >= 40 then { state with EnemigoEstado = Vivo; RedibujarPantalla = true }
        else state
    else state

let procesarTecladoApp key state =
    match key with 
    | ConsoleKey.Escape -> { state with ProgramState = Terminated }
    | _ -> state

let procesarTecladoDeAlien key state =
    if state.AlienEstado = Vivo then 
        match key with  
        | ConsoleKey.UpArrow -> { state with AlienY = max 1 (state.AlienY - 1) }
        | ConsoleKey.DownArrow -> { state with AlienY = min (Console.BufferHeight - 1) (state.AlienY + 1) }
        | ConsoleKey.LeftArrow -> { state with AlienX = max 0 (state.AlienX - 1) }
        | ConsoleKey.RightArrow -> { state with AlienX = min (Console.BufferWidth - 2) (state.AlienX + 1) }
        | ConsoleKey.Spacebar ->
            let nuevoMisil1 = { X = state.AlienX + 2; Y = state.AlienY }
            { state with Misiles = nuevoMisil1 :: state.Misiles; RedibujarPantalla = true }
        | _ -> state
    else state

let procesarTeclado state =
    if Console.KeyAvailable then 
        let k = Console.ReadKey true
        state 
        |> procesarTecladoApp k.Key
        |> procesarTecladoDeAlien k.Key
    else state

let redibujarUI state =
    mostrarMensaje 2 0 ConsoleColor.White (sprintf "PUNTOS: %05d" state.Puntos)
    let corazones = String.replicate (max 0 state.Vidas) "O "
    mostrarMensaje 25 0 ConsoleColor.Red (sprintf "VIDAS: %s" corazones)

let redibujarPantalla state =
    if state.RedibujarPantalla then 
        Console.Clear()
        redibujarUI state
        
        let spriteAlien = if state.AlienEstado = Vivo then "👽" else "💥"
        mostrarMensaje state.AlienX state.AlienY ConsoleColor.Yellow spriteAlien

        state.Misiles |> List.iter (fun m -> mostrarMensaje m.X m.Y ConsoleColor.Yellow  "=>")
        state.MisilesEnemigos |> List.iter (fun m -> mostrarMensaje m.X m.Y ConsoleColor.Cyan "<=")

        let spriteEnemigo = if state.EnemigoEstado = Vivo then "☠️" else "💥"
        mostrarMensaje state.EnemigoX state.EnemigoY ConsoleColor.Red spriteEnemigo

        { state with RedibujarPantalla = false }
    else state

let mostrarLetrerGameOver puntos =
    Console.Clear()
    mostrarMensaje (Console.BufferWidth / 2 - 7) (Console.BufferHeight / 2 - 2) ConsoleColor.Red "GAME OVER"
    mostrarMensaje (Console.BufferWidth / 2 - 10) (Console.BufferHeight / 2) ConsoleColor.White (sprintf "Puntaje Final: %d" puntos)
    mostrarMensaje (Console.BufferWidth / 2 - 16) (Console.BufferHeight / 2 + 2) ConsoleColor.DarkGray "Presiona una tecla para continuar"
    Console.ReadKey(true) |> ignore

let rec mainLoop state =
    let newState =
        state 
        |> actualizarTick
        |> actualizarMisiles
        |> actualizarEnemigo
        |> dispararMisilesEnemigos
        |> actualizarMisilesEnemigos
        |> detectarColisionAlien
        |> detectarColisionEnemigo
        |> resetAlien
        |> resetEnemigo
        |> procesarTeclado
        |> redibujarPantalla
    
    match newState.ProgramState with
    | Running -> 
        Thread.Sleep 25
        mainLoop newState
    | GameOver ->
        mostrarLetrerGameOver newState.Puntos
    | Terminated -> ()

let mostrar (esCargarPartida: bool) =
    Console.Clear()
    Console.CursorVisible <- false
    
    let stateInit = 
        if esCargarPartida then
            let pts, vds = cargarPartida()
            { estadoInicial() with Puntos = pts; Vidas = vds }
        else
            estadoInicial()
    
    mainLoop stateInit  