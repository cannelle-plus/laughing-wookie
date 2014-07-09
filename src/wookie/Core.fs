module Core
open System

let (|InvariantEqual|_|) (str:string) arg = 
  if String.Compare(str, arg, StringComparison.OrdinalIgnoreCase) = 0
    then Some() else None


module String = let notNullOrEmpty = not << System.String.IsNullOrEmpty