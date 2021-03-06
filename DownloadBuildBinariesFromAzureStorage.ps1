<#
-------------------------------------------------------------------------
 Copyright 2013 Microsoft Open Technologies, Inc.
 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at 
   http://www.apache.org/licenses/LICENSE-2.0
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
--------------------------------------------------------------------------
#>
<#
.SYNOPSIS
    Downloads Build binaries from Azure Storage Container to a local folder.

.DESCRIPTION
    Downloads Build binaries blobs from a single storage container to a local folder.  

.EXAMPLE
    .\DownloadBuildBinariesFromAzureStorage `
        -StorageAccountName <myStorageAccountName> -StorageAccountKey <myStorageAccountKey> `
        -ContainerName <myContainerName> -Destination "<myLocalPath>" `
        -BlobNamePrefix "<blobNamePrefix>"
#>

param (
    # The Azure Storage Account Name where the Build Binaries exist
    [Parameter(Mandatory = $true)]
    $StorageAccountName,
    
    # The Azure Storage Account Key
    [Parameter(Mandatory = $true)]
    $StorageAccountKey,
    
    # The Azure Storage Container Name
    [Parameter(Mandatory = $true)]
    $ContainerName,
    
    # The Destination Path on the VM
    [Parameter(Mandatory = $true)]
    $Destination,
    
    # The Blob Name Prefix of the blobs to be downloaded
    [Parameter(Mandatory = $true)]
    $BlobNamePrefix
)

function logAzureError ($errorObj)
{
    $errorMsg = $errorObj.InvocationInfo.InvocationName.ToString() + "  :  " + $errorObj.ToString() + "`n" `
                    + $errorObj.InvocationInfo.PositionMessage.ToString() + "`n" `
                    + "CategoryInfo  :  " + $errorObj.CategoryInfo.ToString() + "`n" `
                    + "FullyQualifiedErrorId  :  " + $errorObj.FullyQualifiedErrorId.ToString()
    
    return $errorMsg
}

function UnZipFile($File, $Destination)
{
    $UnZipfile = "$Destination\7Zip\7za.exe"
    if ((Test-Path $UnZipfile))
    {
        Start-Process -FilePath $UnZipfile -ArgumentList "x ""$File"" -ryo""$Destination""" -PassThru | Wait-Process
    }
    else
    {
        echo "7za.exe not exists for unzip this, build looks for $UnZipfile and failed"
    }
}

Try
{
    md "C:\thiru\test" 
    # Reset the error variable
    $error.clear()

    $cmdletError = ""    
    $continueScriipt = $false
    
    $systemDrive = iex ('$env:SystemDrive')

    # Set the properties for logging
    $logFile = $systemDrive + '\DownloadBuildBinariesFromAzureStorage.log'
    $logFileContent =  "===========================================================================`n" `
                     + "                  DOWNLOAD1 STATUS FOR BUILD - `"" + $BlobNamePrefix + "`"               `n" `
                     + "===========================================================================`n"

    echo $logFileContent 
    $logFileContent = '';

    $userProfile = iex ('$env:USERPROFILE')
    
    $webPICmdMSI = $userProfile + '\Downloads\WebPICmd.msi'


    # Check if Azure PowerShell module exists, else install it
    $azureCmdlet = Get-Module -ListAvailable -Name 'Azure'
    
    if ($azureCmdlet.Name -eq "Azure")
    {
        $logFileContent = $logFileContent + "Windows Azure PowerShell module is already available. `n"
        $continueScript = $true

        echo $logFileContent 
        $logFileContent = '';
    }
    else
    {
        $logFileContent = $logFileContent + "Windows Azure PowerShell is not installed. `n"
        
        echo $logFileContent 
        $logFileContent = '';

        # Check if Web PI Command exists, else install it so that Azure PowerShell can be installed through it
        # Reset the error variable, so that subsequent cmdlet execution will not consider this error
        $error.clear()
                    
        $logFileContent = $logFileContent + "Web PI Command does not exist. Installation will start.... `n"
            
        # Download and Install Web Platform Installer Command Line
        if ([IntPtr]::Size -eq 4)
        {
            (new-object net.webclient).DownloadFile('http://download.microsoft.com/download/7/0/4/704CEB4C-9F42-4962-A2B0-5C84B0682C7A/WebPlatformInstaller_x86_en-US.msi', $webPICmdMSI)
        }
        else
        {
            (new-object net.webclient).DownloadFile('http://download.microsoft.com/download/7/0/4/704CEB4C-9F42-4962-A2B0-5C84B0682C7A/WebPlatformInstaller_amd64_en-US.msi', $webPICmdMSI)
        }

        Start-Process -FilePath $webPICmdMSI -ArgumentList '/quiet' -PassThru | Wait-Process
        
        if ($error.Count -gt 0)
        {
            throw "ERROR OCCURRED: While installing Web PI Command"
        }
        else
        {
            # Set the %PATH% environment variable from the system to the environment variable in the PowerShell session
            # Else, the updated %PATH% will not be available during the script run
            $env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine")

            # Once the installation is successful, delete the downloaded MSI and log files
            Remove-Item $webPICmdMSI
            if ($error.Count -gt 0)
            {
                $logFileContent = $logFileContent + "..... WARNING: While deleting " + $webPICmdMSI + "`n"
                $error.clear()
            }


            $logFileContent = $logFileContent + "....Web PI Command has been successfully installed. `n"
        }

        echo $logFileContent 
        $logFileContent = '';

        $logFileContent = $logFileContent + "Windows Azure PowerShell installation will start..... `n"
        echo $logFileContent 
        $logFileContent = '';

         foreach($level in "Machine","User") {
            [Environment]::GetEnvironmentVariables($level).GetEnumerator() | % {
            #Add Path for variables webPICmd command
            if($_.Name -match 'Path$') { 
                $_.Value = ($((Get-Content "Env:$($_.Name)") + ";$($_.Value)") -split ';' | Select -unique) -join ';'
            }
            $_
            } | Set-Content -Path { "Env:$($_.Name)" }
        }

        # Install WindowsAzurePowerShell
        Start-Process 'WebPICmd' -ArgumentList '/Install /AcceptEULA /Products:WindowsAzurePowershell' -NoNewWindow -PassThru | Wait-Process
        if ($error.Count -gt 0)
        {
            throw "ERROR OCCURRED: While installing Windows Azure PowerShell"
        }

        # Set the %PSModulePath% environment variable from the system to the environment variable in the PowerShell session
        # Else, the updated %PSModulePath% will not be available during the script run
        $env:PSModulePath = [System.Environment]::GetEnvironmentVariable("PSModulePath","Machine")

        $logFileContent = $logFileContent + "....Windows Azure PowerShell has been successfully installed. `n"
        
        $continueScript = $true
        echo $logFileContent 
        $logFileContent = '';
    }    
    
    if ($continueScript)
    {
        # Load the installed Azure module
        # Import-Module Azure
        if ($error.Count -gt 0)
        {
            throw "ERROR OCCURRED: While importing Azure PowerShell module"
        }
        
        # Create the Azure Storage Context using the Account Name and Account Key
        $context = New-AzureStorageContext -StorageAccountName $StorageAccountName -StorageAccountKey $StorageAccountKey
       
        $cmdletError = ""
        
        if ($error.Count -gt 0)
        {
            $cmdletError = logAzureError $error[0]
            $logFileContent = $logFileContent + "`n" + $cmdletError
        }
        else
        {
            # List the Azure Blobs matching the BlobNamePrefix
            $blobsList = Get-AzureStorageBlob -Container $ContainerName -Context $context -Prefix $BlobNamePrefix
            $cmdletError = "" 

            if ($error.Count -gt 0)
            {
                $cmdletError = logAzureError $error[0]
                $logFileContent = $logFileContent + "`n" + $cmdletError
            }
            else 
            {
                # Download each of the listed blobs to the destination folder
                foreach($blob in $blobsList)
                {
                  
                    Echo 'Start downloading the built folder from azure storage'
                    $downloadResult = Get-AzureStorageBlobContent -Container $ContainerName -Context $context -Blob $blob.Name -Destination $Destination -Force
                    Echo 'Finized downloading to the target'
                  
                    $cmdletError = ""
                    Echo 'Start Unzipping the folder'
                    $ZipedFileName = "$Destination\$BlobNamePrefix"
                    UnZipFile $ZipedFileName $Destination
                    Echo 'Finized Unzipping to the target'

                    md "C:\thiru\test1" 

                   if ($error.Count -gt 0)    
                    {
                        $cmdletError = logAzureError $error[0]

                        $logFileContent = $logFileContent + "`n" `
                                            + "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~FAILURE START~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~`n" `
                                            + $cmdletError + "`n" `
                                            + "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~FAILURE END~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~`n" 
                        
                        # Reset the error variable, else for all subsequent downloads, the previous error will be considered
                        $error.clear()
                    }
                    else
                    {
                        $logFileContent = $logFileContent + "`n" `
                                            + "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~SUCCESS START~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~`n" `
                                            + "AbsoluteUri  : " + $downloadResult.ICloudBlob.Uri.AbsoluteUri + "`n" `
                                            + "Blob Name    : " + $downloadResult.Name + "`n" `
                                            + "Blob Type    : " + $downloadResult.BlobType + "`n" `
                                            + "Length       : " + $downloadResult.Length + "`n" `
                                            + "ContentType  : " + $downloadResult.ContentType + "`n" `
                                            + "LastModified : " + $downloadResult.LastModified.UtcDateTime + "`n" `
                                            + "SnapshotTime : " + $downloadResult.SnapshotTime + "`n" `
                                            + "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~SUCCESS END~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~`n"
                    }
                }
            }
        }
    }
}
Catch
{
    $excpMsg = logAzureError($_)
    $logFileContent = $logFileContent + "`n" + $excpMsg + "`n"
}
Finally
{
    # Log the details to log file
    $logFileContent >> $logFile
}