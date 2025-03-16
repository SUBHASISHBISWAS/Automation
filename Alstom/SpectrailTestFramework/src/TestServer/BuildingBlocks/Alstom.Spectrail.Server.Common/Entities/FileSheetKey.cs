#region ©COPYRIGHT

// /*******************************************************************************
//  *   © COPYRIGHT ALSTOM SA 2025 - ALL RIGHTS RESERVED
//  *
//  *  This file is part of the Alstom Spectrail project.
//  *  Unauthorized copying, modification, distribution, or use of this file,
//  *  via any medium, is strictly prohibited.
//  *
//  *  Proprietary and confidential.
//  *  Created and maintained by Alstom for internal use in the Spectrail project.
//  ******************************************************************************/
// 
//  /*******************************************************************************
// AuthorName: SUBHASISH BISWAS
// Email: subhasish.biswas@alstomgroup.com
// FileName: FileSheetKey.cs
// ProjectName: Alstom.Spectrail.Server.Common
// Created by SUBHASISH BISWAS On: 2025-03-15
// Updated by SUBHASISH BISWAS On: 2025-03-15
//  ******************************************************************************/

#endregion

public class FileSheetKey
{
    public string FileName { get; set; } = string.Empty;
    public string SheetName { get; set; } = string.Empty;

    public override bool Equals(object? obj)
    {
        if (obj is not FileSheetKey other) return false;
        return FileName == other.FileName && SheetName == other.SheetName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FileName, SheetName);
    }
}