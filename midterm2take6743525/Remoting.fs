namespace midterm2take6743525

open WebSharper

[<JavaScript>]
module Dto =

    type Books =
        {
            Title: string
            Author: string
            Release: string
        }

    type LoginStatus =
        | Success
        | Failure

module Database =
    open FSharp.Data.Sql
    open Dto

    type DB = SqlDataProvider<
                  ConnectionString = "Server=DESKTOP-29S8MUB\SQLEXPRESS;Database=Test;Trusted_Connection=True;",
                  DatabaseVendor = Common.DatabaseProviderTypes.MSSQLSERVER,
                  UseOptionTypes = true>


    // Ez a fv. szedi ki az adatbázisból a könyveket 
    let AllBooks () =
        let ctx = DB.GetDataContext()
        query {
            for book in ctx.Dbo.Book do
            select book
        }
        |> Seq.map (fun book ->
            {
                Title = book.Title 
                Author = book.Author
                Release = book.ReleaseDate
            }
        )
        |> Seq.toList


    // Ez a fv. felelős az adatbázisba való beszúrásért, nagy része SQLProvider-es dokumentációból van kimásolva, majd itt működővé alakítva
    let AddNewBook(title: string, author: string, release: string) =
        
        // SQLProvideres + a mi általunk írt lekérdezős példák alapján:
        let ctx = DB.GetDataContext()
        
        let Book = ctx.Dbo.Book
        
        let row = Book.Create()
        
        row.Author <- author
        row.Title <- title
        row.ReleaseDate <- release
        ctx.SubmitUpdates()

module Server =
    open WebSharper.Web
    open Dto

    // Sending password in cleartext - you should only do this via HTTPS
    [<Rpc>]
    let DoLogin username (password: string) =
        let ctx = Remoting.GetContext()
        // TODO: check user, here we let them all pass
        async {
            do! ctx.UserSession.LoginUser username
            return LoginStatus.Success
        }

    [<Rpc>]
    let GetAllBooks () =
        async {
            return Database.AllBooks ()
        }

    // Rpc hívás ami az adatbázisba beszúró fv.-t hívja
    [<Rpc>]
    let AddNewBook(title: string, author: string, release: string) =
            Database.AddNewBook (title, author, release)
        