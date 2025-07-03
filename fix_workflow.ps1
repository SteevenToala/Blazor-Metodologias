$filePath = "c:\metodologias\blazor\MyCleanApp.Infrastructure\Services\PromocionWorkflowService.cs"
$content = Get-Content $filePath -Raw
$content = $content -replace 'WorkflowResult\.Success\(', 'WorkflowResult.CreateSuccess('
$content = $content -replace 'WorkflowResult\.Error\(', 'WorkflowResult.CreateError('
Set-Content $filePath $content
