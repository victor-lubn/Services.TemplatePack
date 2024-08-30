function Set-Module {

    <#
      .SYNOPSIS
      Short description.

      .DESCRIPTION
      Description

      .PARAMETER Parameter1
      Parameter1 description.

      .INPUTS
      None. You cannot pipe objects to Add-Extension.

      .OUTPUTS
      System.Object. Returns object converted from JSON configuration.

      .EXAMPLE
      PS> Set-Module -Parameter1 "value"
    #>

    [CmdletBinding()]

    Param(
        [Parameter(Mandatory)]
        [String]$Parameter1
    )

    try {
        Write-Host "`n`n`n####################---<<ModuleName>>---####################"
        Set-Something
    } catch {
        $errorMessage = $_.Exception
        $errorType = $_.Exception.GetType().fullname
        Write-Error "Unexpected error: <<Function>> has been failed. $errorMessage"
        Write-Host "Error type: $errorType"
        exit 1
    } finally {
        Write-Host "####################---<<ModuleName>>---####################"
    }

}
