using SIGL_Cadastru.Mobile.Models.Migrations;

namespace SIGL_Cadastru.Mobile.Services.Migrations;

/// <summary>
/// Interface for migration operations
/// </summary>
public interface IMigrationService
{
    Task<MigrationResult> MigrateClientsAsync(ClientMigrationCommand command);
    Task<MigrationResult> MigrateRequestsAsync(MigrateCadastralRequestCommand command);
    Task<ExctractDataResult> UploadDatabaseAsync(Stream dbFileStream, string fileName);
}
