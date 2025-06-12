public class RequisitoNivel
{
    public int Id { set; get; }
    public string Nombre { set; get; } = "";
    public int ValorRequerido { set; get; }
    public override string ToString()
    {
        return $"Id: {Id}, Nombre: {Nombre}, ValorRequerido: {ValorRequerido}";
    }
}