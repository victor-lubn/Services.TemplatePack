<#
  .SYNOPSIS
  Short description

  .DESCRIPTION
  Full description

  .INPUTS
  None. You cannot pipe objects to the script.

  .OUTPUTS
  None. Script does not generate any output.

  .EXAMPLE
  PS> .\Set-Script.ps1

  .NOTES
  Some notes
#>



#-----------------------------------------------------------------------
# Auxiliary functions to process settings
#-----------------------------------------------------------------------
function Set-LocalFunction (Parameter1) {
    Set-Something
}



#-----------------------------------------------------------------------
# Modules import
#-----------------------------------------------------------------------
Import-Module "$PSScriptRoot\modules\Set-ModuleName.psm1" -Force



#-----------------------------------------------------------------------
# Mandatory Environment Variables
#-----------------------------------------------------------------------
$environment = $env:ENVIRONMENT

if (!$env:ENVIRONMENT) { Write-Error "Environment variable ENVIRONMENT doesn't exist." }



#-----------------------------------------------------------------------
# Variables - script reads variables from environment variables, if
# variables cannot be found, script takes values from configuration.
#-----------------------------------------------------------------------
$deploymentConfiguration = Read-Configuration -ConfigurationPath $deploymentConfigurationPath

if ($env:DIGITAL_SUBSCRIPTION_ID) {
    $digitalSubscriptionId = $env:DIGITAL_SUBSCRIPTION_ID
} else {
    $digitalSubscriptionId = $deploymentConfiguration.'Digital Subscription ID'.$environment
}
  
  
  
#-----------------------------------------------------------------------
# Script execution
#-----------------------------------------------------------------------
Set-Script
