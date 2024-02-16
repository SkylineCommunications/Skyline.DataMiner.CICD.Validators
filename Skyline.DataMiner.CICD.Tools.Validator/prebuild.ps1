$nuget_package_installation_directory = $args[0]
$csprojPath = "./Skyline.DataMiner.CICD.Tools.Validator.csproj"

# Load the csproj file as XML
$xml = [xml](Get-Content $csprojPath)

# Find the PackageReference element
$packageReference = $xml.Project.ItemGroup.PackageReference | Where-Object { $_.Include -eq "Skyline.DataMiner.XmlSchemas.Protocol" }

if ($packageReference)
{
    # Extract the version attribute value
    $version = $packageReference.Version

    # Use the extracted version in your pre-build event
	$sourceFilePath = $nuget_package_installation_directory +"skyline.dataminer.xmlschemas.protocol/$version/content/Skyline/XSD/uom.xsd"
    $destinationFilePath = "./Resources/uom.xsd"
	Copy-Item -Path $sourceFilePath -Destination $destinationFilePath -Force
}
else
{
    Write-Host "PackageReference for Skyline.DataMiner.XmlSchemas.Protocol not found in $csprojPath"
}