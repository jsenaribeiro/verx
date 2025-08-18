namespace FluxoCaixa.Domain.Commons;

public abstract class Entity
{
   public Guid Id { get; set; } = Guid.Empty;
}