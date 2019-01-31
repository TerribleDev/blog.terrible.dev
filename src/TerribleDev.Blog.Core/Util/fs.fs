module fs

open System.IO

    let getFileInfo (filePath:string) = 
        let fileInfo = FileInfo(filePath)
        async {
            let! text = File.ReadAllTextAsync(fileInfo.FullName) |> Async.AwaitTask
            return (text, fileInfo)
        }