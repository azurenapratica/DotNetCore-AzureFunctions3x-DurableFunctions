using System;
public class Produto 
{
    public Guid Id { get { return Guid.NewGuid(); } }
    public string Nome { get; set; }
    public decimal Valor { get; set; }
}