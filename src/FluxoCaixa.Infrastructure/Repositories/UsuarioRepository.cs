using FluxoCaixa.Domain.Usuarios;

namespace FluxoCaixa.Infrastructure.Repositories;

public class UsuarioRepository : AbstractRepository<Usuario>, IUsuarioRepository
{
   public UsuarioRepository(IServiceProvider provider) : base(provider) { }
}