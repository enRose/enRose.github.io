public async Task UpdateContactDetails(
  Guid userId, ContactDetails contactDetails)
{
    var userSub = await MarvelStudio.GetUserSubscription(userId).ConfigureAwait(false);

    userSub.Contact.Address = contactDetails.Address ??
        userSub.Contact.Address;

    userSub.Contact.Number = contactDetails.Number ??
        userSub.Contact.Number;

    userSub.Contact.Email = contactDetails.Email ?? 
        userSub.Contact.Email;

    // Option 1: this is how we normally do it:
    var tasks2 = new List<Task>()
    {
        MarvelStudio.SaveUserSubscription(userId, userSub)
    };

    if (contactDetails.Address != null) 
    {
        tasks2.Add(UpdateAddress(userId, contactDetails.Address));
    }

    if (!string.IsNullOrWhiteSpace(contactDetails.Number))
    {
        tasks2.Add(Customer.UpdatePhone(userId, contactDetails.Number));
    }

    if (!string.IsNullOrWhiteSpace(contactDetails.Email))
    {
        tasks2.Add(Customer.UpdateEmail(userId, contactDetails.Email));
    }

    await Task.WhenAll(tasks2).ConfigureAwait(false);


    // Option 2: closure and thunk
    var c = new Dictionary<Func<bool>, Func<Task>> {
        {
            () => contactDetails.Address != null,
            () => UpdateAddress(userId, contactDetails.Address)
        },
        {
            () => !string.IsNullOrWhiteSpace(contactDetails.Number),
            () => Customer.UpdatePhone(userId, contactDetails.Number)
        },
        {
            () => !string.IsNullOrWhiteSpace(contactDetails.Email),
            () => Customer.UpdateEmail(userId, contactDetails.Email)
        }
    };

    var tasks = c
        .Where(x => x.Key())
        .Select(x => x.Value())
        .Append(MarvelStudio.SaveUserSubscription(userId, userSub));

    await Task.WhenAll(tasks).ConfigureAwait(false);
}
