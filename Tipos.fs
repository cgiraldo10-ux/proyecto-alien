module App.Tipos

type ProgramState = Running | Terminated | GameOver  | Paused 

type Misil = { X: int; Y: int }
type EstadoDeSprite = Vivo | Muerto

type State = {
    ProgramState: ProgramState
    AlienX: int
    AlienY: int
    AlienEstado: EstadoDeSprite
    ColisionAlien: int 
    RedibujarPantalla: bool
    Tick: int
    Misiles: Misil list
    EnemigoX: int
    EnemigoY: int
    EnemigoDir: int
    EnemigoEstado: EstadoDeSprite
    ColisionEnemigo: int
    MisilesEnemigos: Misil list

    Puntos: int
    Vidas: int
}

type OpcionMenu = NuevoJuego | CargarPartida | Salir
type EstadosEnrutador = MostrarMenu | Terminar