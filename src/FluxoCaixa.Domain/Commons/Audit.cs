using System.Diagnostics.CodeAnalysis;

namespace FluxoCaixa.Domain.Commons;

/// <summary>
/// Auditoria de mudanças
/// </summary>
/// <param name="when">Data e hora</param>
/// <param name="where">Campo da entidade</param>
/// <param name="what">Valor da entidade (opcional)</param>
/// <param name="who">Nome da entidade</param>
/// <param name="how">operação = Create|Update|Delete</param>
public record Audit(DateTime when, string where, string what, string who, CRUD how);