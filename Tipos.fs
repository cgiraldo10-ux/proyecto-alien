module App.Tipos

type ProgramState = Running | Terminated | GameOver

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
    // Nuevas propiedades añadidas
    Puntos: int
    Vidas: int
}

type OpcionMenu = NuevoJuego | CargarPartida | Salir
type EstadosEnrutador = MostrarMenu | Terminar