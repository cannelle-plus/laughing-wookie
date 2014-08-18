module Core
open System

type executionResult<'TEvent> =
    | Success of 'TEvent
    | Failure of string list
   

let (|InvariantEqual|_|) (str:string) arg = 
  if String.Compare(str, arg, StringComparison.OrdinalIgnoreCase) = 0
    then Some() else None


module String = let notNullOrEmpty = not << System.String.IsNullOrEmpty