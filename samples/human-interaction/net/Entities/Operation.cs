public class Operation
{
    public string Id { get; set; }

    public string Tipo { get; set; } = default!;

    public string CuentaOrigen { get; set; } = default!;

    public string CuentaDestino { get; set; } = default!;
    
    public decimal Monto { get; set;} = default!;
}