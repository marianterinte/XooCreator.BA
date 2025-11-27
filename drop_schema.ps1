Add-Type -Path "D:/WORK/MyWork/Alchimalia/BA/XooCreator.BA/XooCreator.DbScriptRunner/bin/Debug/net8.0/Npgsql.dll"
$connectionString = "Host=localhost;Port=5432;Database=xoo_db;Username=postgres;Password=admin"
$conn = New-Object Npgsql.NpgsqlConnection($connectionString)
$conn.Open()
try {
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = 'DROP SCHEMA IF EXISTS alchimalia_schema CASCADE;'
    $cmd.ExecuteNonQuery() | Out-Null
    $cmd.Dispose()
}
finally {
    $conn.Close()
}
Write-Output 'Schema alchimalia_schema dropped (if existed).'
