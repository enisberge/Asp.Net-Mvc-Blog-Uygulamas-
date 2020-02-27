namespace MyEvernote.Entities.Messages
{
    public enum ErrorMessageCode
    {
        //bu kısımda errorler ve kodları olacak

        UsernameAlreadyExist = 101,
        EmailAlreadyExist = 102,
        UserIsNotActive = 151,
        UsernameOrPassWrong = 152,
        CheckYourEmail = 153,
        UserAlreadyActive = 154,
        ActivateIdDoesNotExist = 155,
        UserNotFound = 156,
        ProfileCouldNotUpdate = 157,
        UserCouldNotRemove = 158,
        UserCouldNotFind = 159,
        UserCouldNotInserted = 160,
        UserCouldNotUpdated = 161,
    }
}
