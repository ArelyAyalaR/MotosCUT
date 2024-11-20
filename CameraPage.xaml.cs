using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZXing.Net.Maui;

namespace ProyectoMotos
{
    public partial class CameraPage : ContentPage
    {
        private bool isProcessing = false;
        private string qrCodeData;

        public CameraPage()
        {
            InitializeComponent();
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        private async void CameraBarcodeReaderView_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
        {
            if (isProcessing) return;

            isProcessing = true;

            foreach (var barcode in e.Results)
            {
                await Dispatcher.DispatchAsync(async () =>
                {
                    qrCodeData = barcode.Value;

                    await DisplayAlert("Código QR Detectado", qrCodeData, "OK");

                    CameraReader.IsDetecting = false;
                    isProcessing = false;

                    if (!string.IsNullOrEmpty(qrCodeData))
                    {
                        var qrIdMatch = Regex.Match(qrCodeData, @"QR ID:\s*(\d+)");

                        if (qrIdMatch.Success)
                        {
                            int qrId = int.Parse(qrIdMatch.Groups[1].Value);

                            int userId = await GetUserIdByQRId(qrId);

                            if (userId != -1) 
                            {
                                var lastRecord = await GetLastEntryAndExitRecord(userId);

                                Debug.WriteLine($"Último registro: EaEID = {lastRecord.EaEID}, RecordType = {lastRecord.RecordType}, FirstName = {lastRecord.FirstName}");

                                if (lastRecord.EaEID != 0 && lastRecord.RecordType == 0)
                                {
                                    await CreateEntryAndExitRecord(userId, 1); 
                                    await DisplayAlert("Bienvenido", $"Hola, {lastRecord.FirstName}", "OK");
                                }
                                else if (lastRecord.RecordType == 1 || lastRecord.EaEID == 0)
                                {
                                    if (lastRecord.EaEID != 0)
                                    {
                                        await UpdateRecordTypeToExit(lastRecord.EaEID);
                                    }
                                    await CreateEntryAndExitRecord(userId, 0); 
                                    await DisplayAlert("Hasta luego", "Gracias por tu visita.", "OK");
                                }
                            }
                        }
                    }
                });
            }
        }

        private async Task<int> GetUserIdByQRId(int qrId)
        {
            string connectionString = "Server=motocut.cfko0iqhcsi0.us-east-1.rds.amazonaws.com;Database=motosv3;User ID=admin;Password=motoCut$2024DB";
            int userId = -1;

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = "SELECT UserID FROM qrcode WHERE QrID = @qrId";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@qrId", qrId);

                        var result = await command.ExecuteScalarAsync();
                        if (result != null)
                        {
                            userId = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el UserID: {ex.Message}");
            }

            return userId;
        }

        private async Task<(int EaEID, int RecordType, string FirstName)> GetLastEntryAndExitRecord(int userId)
        {
            string connectionString = "Server=motocut.cfko0iqhcsi0.us-east-1.rds.amazonaws.com;Database=motosv3;User ID=admin;Password=motoCut$2024DB";
            (int EaEID, int RecordType, string FirstName) lastRecord = (0, -1, string.Empty);

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = @"
            SELECT EaEID, RecordType, FirstName
            FROM entryandexit e
            JOIN users u ON e.UserID = u.UsersID
            WHERE e.UserID = @userId
            ORDER BY TimeStamp DESC
            LIMIT 1";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                lastRecord = (
                                    EaEID: reader.GetInt32("EaEID"),
                                    RecordType: reader.GetInt32("RecordType"),
                                    FirstName: reader.GetString("FirstName")
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el último registro: {ex.Message}");
            }

            return lastRecord;
        }

        private async Task UpdateRecordTypeToExit(int eaEID)
        {
            string connectionString = "Server=motocut.cfko0iqhcsi0.us-east-1.rds.amazonaws.com;Database=motosv3;User ID=admin;Password=motoCut$2024DB";

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = "UPDATE entryandexit SET RecordType = 1 WHERE EaEID = @eaEID";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@eaEID", eaEID);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el RecordType: {ex.Message}");
            }
        }

        private async Task CreateEntryAndExitRecord(int userId, int recordType)
        {
            try
            {
                string connectionString = "Server=motocut.cfko0iqhcsi0.us-east-1.rds.amazonaws.com;Database=motosv3;User ID=admin;Password=motoCut$2024DB";
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = "INSERT INTO entryandexit (UserID, RecordType, PreviousHash, CurrentHash) " +
                                   "VALUES (@userId, @recordType, @previousHash, @currentHash)";
                    var command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@userId", userId);
                    command.Parameters.AddWithValue("@recordType", recordType);
                    command.Parameters.AddWithValue("@previousHash", "TEMP");  // Ojo Arely, aqui meto valores TEMP para evitar que la
                    command.Parameters.AddWithValue("@currentHash", "TEMP");   // BD truene por que no permite NULL 

                    await command.ExecuteNonQueryAsync();
                    Debug.WriteLine("Registro insertado correctamente.");
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine($"Error al insertar registro en la base de datos: {ex.Message}");
                await DisplayAlert("Error", "No se pudo guardar el registro en la base de datos. Intenta de nuevo.", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error inesperado: {ex.Message}");
                await DisplayAlert("Error", "Ha ocurrido un error inesperado. Intenta de nuevo.", "OK");
            }
        }

        private async void OnScanAgainClicked(object sender, EventArgs e)
        {
            CameraReader.IsDetecting = true;
            isProcessing = false;  
        }
    }
}
