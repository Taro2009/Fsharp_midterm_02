namespace midterm2take6743525

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html





[<JavaScript>]
module Client =

    // Óráról megmaradt kódrészlet, az appban nem fontos

    open WebSharper.Forms

    let LoginForm () =
        Form.Return (fun user pass msg -> user, pass, msg)
        <*> (Form.Yield ""
            |> Validation.IsNotEmpty "Must enter a username")
        <*> (Form.Yield ""
            |> Validation.IsNotEmpty "Must enter a password")
        <*> (Form.Yield "")
        |> Form.WithSubmit
        |> Form.Run (fun (u, p, msg) ->
            async {
                let! status = Server.DoLogin u p
                match status with
                | Dto.LoginStatus.Success ->
                    JS.Alert "User logged in"
                    JS.Window.Location.Replace "/protected"
                | Dto.LoginStatus.Failure ->
                    JS.Alert "Failed to log user in"
            }
            |> Async.Start
        )
        |> Form.Render (fun user pass msg submit ->
            let view =
                msg.View
                |> View.Map (fun msg -> msg + ":" + msg)
            div [] [
                div [] [label [] [text "Username: "]; Doc.Input [] user]
                div [] [label [] [text "Password: "]; Doc.PasswordBox [] pass]
                // Silly addition to demo a reactive dependent UI element
                div [] [label [] [text "Password: "]; Doc.Input [] msg]
                div [] [textView view]
                Doc.Button "Log in" [] submit.Trigger
                div [] [
                    Doc.ShowErrors submit.View (fun errors ->
                        errors
                        |> Seq.map (fun m -> p [] [text m.Text])
                        |> Doc.Concat)
                ]
            ]
        )

        // Órai kódrészlet vége


















    let Main () =
        let rvInput = Var.Create ""

        let NewBookTitle = Var.Create ""
        let NewBookAuthor = Var.Create ""
        let NewBookDate = Var.Create ""

        // Ez kezeli le a gomb nyomását, amivel lekérjük a könyveket. A szerver oldali Server.GetAllBooks() fog segíteni nekünk ebben.
        let submit = Submitter.CreateOption rvInput.View
        let vReversed = 
            submit.View.MapAsync(function
                | None -> async { return "" }
                | Some input ->
                    async {
                        let F (book: Dto.Books) =
                            sprintf "%s, %s, %s " book.Title book.Author book.Release
                        let! res = Server.GetAllBooks()
                        let res = res |> List.fold (fun acc book -> acc + F book + "; ") ""
                        return res
                    }
            )

        // Itt pakoljuk hozzá a UI-hoz azokat az elemeket amik majd a működéshez kellenek
        div [] [


            br [] []
            h1 [] [text "Könyv adatbázisos app - Horváth Barnabás"]
            br [] []
            
            Doc.Button "Az összes könyv lekérdezése" [] submit.Trigger
            br [] []
            br [] []
            br [] []

            div [attr.``class`` "jumbotron"] [h3 [] [textView vReversed]]

            hr [] []
            div [] [
                // Új könyv hozzáadó form
                h2 [] [text "Új könyv hozzáadása"]
                h3 [] [text "Cím:"]
                Doc.Input [] NewBookTitle
                h3 [] [text "Író:"]
                Doc.Input [] NewBookAuthor
                h3 [] [text "Kiadás dátuma:"]
                Doc.Input [] NewBookDate
                br [] []
                br [] []
                Doc.Button "Adatbázisba mentés" [] (fun _ ->
                Server.AddNewBook(NewBookTitle.Value, NewBookAuthor.Value, NewBookDate.Value)
            )

            ]

            hr [] []



        ]
