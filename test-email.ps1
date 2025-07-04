# Script para probar el envío de correo
$testBody = @{
    "to" = "steevenpfb@gmail.com"
    "decision" = "APROBADO"
    "observaciones" = "Excelente rendimiento académico"
    "nombreDocente" = "Juan Pérez"
    "nivelSolicitado" = "Profesor Titular"
} | ConvertTo-Json

Write-Host "Enviando correo de prueba..."
Write-Host "Cuerpo del request: $testBody"

try {
    $response = Invoke-RestMethod -Uri "http://localhost:5015/api/test/enviar-correo" -Method POST -Body $testBody -ContentType "application/json"
    Write-Host "Respuesta: $($response | ConvertTo-Json -Depth 10)"
}
catch {
    Write-Host "Error: $($_.Exception.Message)"
    Write-Host "Status Code: $($_.Exception.Response.StatusCode)"
    Write-Host "Response: $($_.Exception.Response | ConvertTo-Json -Depth 10)"
}
