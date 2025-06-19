public static class OperationFactory
{
    public static Operation Create(string tipo, string cuentaOrigen,
                                    string cuentaDestino, decimal monto) {
        return new Operation
        {
            Id = Guid.NewGuid().ToString(),
            Tipo = tipo,
            CuentaOrigen = cuentaOrigen,
            CuentaDestino = cuentaDestino,
            Monto = monto
        };
    }
}