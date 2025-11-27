function Convert-ToNetworkOrder {
    param([byte[]]$Bytes)
    [Array]::Reverse($Bytes, 0, 4)
    [Array]::Reverse($Bytes, 4, 2)
    [Array]::Reverse($Bytes, 6, 2)
}

function Convert-FromNetworkOrder {
    param([byte[]]$Bytes)
    [Array]::Reverse($Bytes, 0, 4)
    [Array]::Reverse($Bytes, 4, 2)
    [Array]::Reverse($Bytes, 6, 2)
}

function New-DeterministicGuid {
    param(
        [string]$NamespaceGuid,
        [string]$Name
    )

    $namespaceBytes = ([Guid]$NamespaceGuid).ToByteArray()
    Convert-ToNetworkOrder $namespaceBytes

    $nameBytes = [System.Text.Encoding]::UTF8.GetBytes($Name)
    $buffer = New-Object byte[] ($namespaceBytes.Length + $nameBytes.Length)
    [Array]::Copy($namespaceBytes, 0, $buffer, 0, $namespaceBytes.Length)
    [Array]::Copy($nameBytes, 0, $buffer, $namespaceBytes.Length, $nameBytes.Length)

    $sha1 = [System.Security.Cryptography.SHA1]::Create()
    try {
        $hash = $sha1.ComputeHash($buffer)
    }
    finally {
        $sha1.Dispose()
    }

    $guidBytes = New-Object byte[] 16
    [Array]::Copy($hash, 0, $guidBytes, 0, 16)
    $guidBytes[6] = ($guidBytes[6] -band 0x0F) -bor 0x50
    $guidBytes[8] = ($guidBytes[8] -band 0x3F) -bor 0x80

    Convert-FromNetworkOrder $guidBytes
    return (New-Object Guid (,$guidBytes)).ToString()
}

function Get-GuidLiteral {
    param(
        [string]$NamespaceGuid,
        [string]$Name
    )
    return "'" + (New-DeterministicGuid -NamespaceGuid $NamespaceGuid -Name $Name) + "'"
}

