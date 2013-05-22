﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Signum.Entities.Authorization
{
    public enum AuthMessage
    {
        [Description(@"{0} cycles have been found in the graph of Roles due to the relationships:")]
        _0CyclesHaveBeenFoundInTheGraphOfRolesDueToTheRelationships,
        [Description("{0} rules")]
        _0Rules,
        [Description("{0} rules for {1}")]
        _0RulesFor1,
        [Description("Access to Facade Method '{0}' is not allowed")]
        AccessToFacadeMethod0IsNotAllowed,
        [Description("Add condition")]
        AuthAdmin_AddCondition,
        [Description("Choose a condition")]
        AuthAdmin_ChooseACondition,
        [Description("Remove condition")]
        AuthAdmin_RemoveCondition,
        AuthorizationCacheSuccessfullyUpdated,
        ChangePassword,
        [Description("Current password")]
        ChangePasswordAspx_ActualPassword,
        [Description("Change password")]
        ChangePasswordAspx_ChangePassword,
        [Description("Confirm new password")]
        ChangePasswordAspx_ConfirmNewPassword,
        [Description("New password")]
        ChangePasswordAspx_NewPassword,
        [Description("Write your current password and the new one")]
        ChangePasswordAspx_WriteActualPasswordAndNewOne,
        ConfirmNewPassword,
        [Description("The email must have a value")]
        EmailMustHaveAValue,
        EmailSent,
        EnterTheNewPassword,
        [Description("Entity Group")]
        EntityGroupsAscx_EntityGroup,
        [Description("Overriden")]
        EntityGroupsAscx_Overriden,
        [Description("Expected a user logged")]
        ExpectedUserLogged,
        ExpiredPassword,
        [Description("Your password has expired. You should change it")]
        ExpiredPasswordMessage,
        [Description("Allow")]
        FacadeMethodsAscx_Allow,
        [Description("Deny")]
        FacadeMethodsAscx_Deny,
        [Description("Facade Method")]
        FacadeMethodsAscx_FacadeMethod,
        [Description("Overriden")]
        FacadeMethodsAscx_Overriden,
        [Description("Forgot your password? Enter your login email below. We will send you an email with a link to reset your password.")]
        ForgotYourPassword,
        IHaveForgottenMyPassword,
        IncorrectPassword,
        [Description("Introduce your username and password")]
        IntroduceYourUserNameAndPassword,
        InvalidUsernameOrPassword,
        [Description("New:")]
        Login_New,
        [Description("Password:")]
        Login_Password,
        [Description("Repeat:")]
        Login_Repeat,
        [Description("Username:")]
        Login_UserName,
        [Description("user")]
        Login_UserName_Watermark,
        [Description("Login")]
        LoginEnter,
        NewPassword,
        [Description("Not allowed to save this {0} while offline")]
        NotAllowedToSaveThis0WhileOffline,
        [Description("Not authorized to {0} the {1} with Id {2}")]
        NotAuthorizedTo0The1WithId2,
        [Description("Not authorized to Retrieve '{0}'")]
        NotAuthorizedToRetrieve0,
        [Description("Not authorized to Save '{0}'")]
        NotAuthorizedToSave0,
        NotUserLogged,
        [Description("Allow")]
        OperationsAscx_Allow,
        [Description("DB Only")]
        OperationsAscx_DBOnly,
        [Description("None")]
        OperationsAscx_None,
        [Description("Operation")]
        OperationsAscx_Operation,
        [Description("Overriden")]
        OperationsAscx_Overriden,
        Password,
        PasswordChanged,
        [Description("The given password doesn't match the current one")]
        PasswordDoesNotMatchCurrent,
        [Description("The password has been changed successfully")]
        PasswordHasBeenChangedSuccessfully,
        [Description("The password must have a value")]
        PasswordMustHaveAValue,
        [Description("Your password is near to expired")]
        PasswordNearExpired,
        PasswordsAreDifferent,
        PasswordsDoNotMatch,
        [Description("Allow")]
        PermissionsAscx_Allow,
        [Description("Deny")]
        PermissionsAscx_Deny,
        [Description("Overriden")]
        PermissionsAscx_Overriden,
        [Description("Permission")]
        PermissionsAscx_Permission,
        [Description("Please, {0} into your account")]
        Please0IntoYourAccount,
        [Description("Please, enter your chosen new password")]
        PleaseEnterYourChosenNewPassword,
        [Description("Modify")]
        PropertiesAscx_Modify,
        [Description("None")]
        PropertiesAscx_None,
        [Description("Overriden")]
        PropertiesAscx_Overriden,
        [Description("Property")]
        PropertiesAscx_Property,
        [Description("Read")]
        PropertiesAscx_Read,
        [Description("Allow")]
        QueriesAscx_Allow,
        [Description("Deny")]
        QueriesAscx_Deny,
        [Description("Overriden")]
        QueriesAscx_Overriden,
        [Description("Query")]
        QueriesAscx_Query,
        Remember,
        RememberMe,
        [Description("Reset Password")]
        ResetPassword,
        ResetPasswordCode,
        [Description("A confirmation code to reset your password has been sent to the email account {0}")]
        ResetPasswordCodeHasBeenSent,
        [Description("Your password has been successfully changed")]
        ResetPasswordSuccess,
        Save,
        TheConfirmationCodeThatYouHaveJustSentIsInvalid,
        [Description("The password must have between 7 and 15 characters, each of them being a number 0-9 or a letter")]
        ThePasswordMustHaveBetween7And15CharactersEachOfThemBeingANumber09OrALetter,
        [Description("There has been an error with your request to reset your password. Please, enter your login.")]
        ThereHasBeenAnErrorWithYourRequestToResetYourPasswordPleaseEnterYourLogin,
        [Description("There's not a registered user with that email address")]
        ThereSNotARegisteredUserWithThatEmailAddress,
        [Description("The specified passwords don't match")]
        TheSpecifiedPasswordsDontMatch,
        TheUserStateMustBeDisabled,
        [Description("Create")]
        TypesAscx_Create,
        [Description("Modify")]
        TypesAscx_Modify,
        [Description("None")]
        TypesAscx_None,
        [Description("Operations")]
        TypesAscx_Operations,
        [Description("Overriden")]
        TypesAscx_Overriden,
        [Description("Properties")]
        TypesAscx_Properties,
        [Description("Queries")]
        TypesAscx_Queries,
        [Description("Read")]
        TypesAscx_Read,
        [Description("Type")]
        TypesAscx_Type,
        User,
        [Description("Username {0} is not valid")]
        Username0IsNotValid,
        [Description("The user name must have a value")]
        UserNameMustHaveAValue,
        View,
        [Description("We received a request to create an account. You can create it following the link below:")]
        WeReceivedARequestToCreateAnAccountYouCanCreateItFollowingTheLinkBelow,
        [Description("You must repeat the new password")]
        YouMustRepeatTheNewPassword,
        [Description("User {0} is disabled")]
        User0IsDisabled,
    }

    public enum AuthEmailMessage
    {
        [Description(@"<p>You recently requested a new password</p><p>Your username is: {0}</p><p>You can reset your password by following the link below</p><a href=""{1}"">{1}</a>")]
        ResetPasswordRequestBody,
        [Description("Reset password request")]
        ResetPasswordRequestSubject
    }

    public enum AuthAdminMessage
    {
        [Description("{0} of {1}")]
        _0of1,
        Nothing,
        Everything,
    }

}
