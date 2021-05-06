// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using Newtonsoft.Json;

namespace Dolittle.Runtime.Embeddings.Processing.for_CompareProjectionStates.when_comparing
{
    public class and_the_states_arent_valid_json
    {
        static CompareProjectionStates comparer;
        static ProjectionState left;
        static ProjectionState right;

        Establish context = () =>
        {
            left = new ProjectionState("ị̷̕ṃ̵̖̂̿ ̴̞̝͔̇͐͑n̶͙̄o̴̙̔̇t̷͉̥̯̀͋͠ ̷̨͇̖͐v̴̧͊͛̉ȁ̶̡̾̚ḻ̵̩̯͌̂͐ì̶̟́ḍ̷̔͝ ̴̹̃͌j̴̨̗̒̍s̶̯͉̐o̴̫̯͊n̴̜̉");
            right = new ProjectionState("n̷̮͎͆͗͒͐̂̀͂͒̍͑͒́́͊̏̂͠e̶̢̨̢̡̧̢̨̨̳̮͈̤̭̩͈̭̠̲̬̬̤̪̥̤͚̜̻̘̲͍̱͇̹̗̝̩̹͔̭̥͙͔̤͚͇̘̜̮̣̞̫͕͖͉͉̜͖͙̝͓̰̩͇̺͍̱̤̝͙͇͚̯̫̠̲̤̞͖̰͔͍̩̙̯̗̣̹̠̮͓̰̦̣̹̰͚̥̜͚̭̜̻͙͙͚͈̱͙͒̃́̔̀̾̈́̉͘̕͜͜͜͜͜͝ͅį̶̨̡̨̡̢̨̡̧̛̟̬̪̲̹̲̲̗̭͔̗͎̭̦̻̳̦̬͉̥̤̝͚̟̰̗̹͙̱͍̣̰̲̖̻̦̟̘̞̭̘̻̱̖̪̗̭̻̥̮̰̦͕̼͉̻̳̟͇̘̜͖͇͇̞͙̠̰̘̙̟̖͎̫̯̤͙͈͈̥̙̲̞̜̫͍̹̇͌͑͛́͗͊͌̑̃͑̈́̓̽̀̓̍̌̏̍̌̈́̐̍́͛̒͛̅͛̎̈́̈́̂̔͌̽̏̒̒̈́̍̉̊̃̊͗͊͗̒̾̆̉̾͋̄̈̿̈́̂͊͑̈́͒̒̔̀̽̿̀̔͊̈́̏̒͐̄̉̋͛̈̐̈͗̓͌̚̚͘̕͘͜͝͝͠͝͝͝͝͝ͅͅͅͅͅt̸̡̧̝̼̤̻̹̗̱̱͚̫̩̪̳̹͈̟͙̩͖͖͚̟͈͙̮͚̘͉̭̬̲̜̘͖̱͚̰͚͒͒̿̽́̂̓͑̉̿̊̈̓̓̈͑̏̾̔̌̾̑͌̿̍͗̑͆̈́͊͋̓̏̈̕͜͜͝͠ͅh̴̨̢̨̡̨̢̨̨̡̡̛͕̠͔̱͎̰̞̦̫͉͎̲͍̣̙̠͎̯̖̘̳̫͇̣͙͔̥̪̠̻̝͇̗̩͈̬͉̭̦̞̗̭̥͖̦̖̝̮̳͕̟͉̲̲̜͖̥̥̯̪̤̯͕͔̤̰̯͕̱̠̻͓̝̟͔̣͙̖͖̺̰͉̮̞͇̱̦͕̯̓́͆̏́̇͒̈͂̃͆͌́̋̎̓̊́̐̓̎͋͑͊̓̄͂́̅̓͗̒͋̀̀̆̀̿̐̃͋̔̂͒̊̎͆́̏͂͘͜͜͝͝ͅͅͅͅȇ̴̡̡̡̡̧̛͉̰̩̜̹͇̯̤͔̣͔̹̯̞͔̗̝̼̺̜͙̰͖̫̥̳͚͔̺͍̦͖͖͙̣̭̟̳͖͇̤̙̜͖͇̥̼̝͓̹͔͇̹͕̖̜̟̥̩̳͔̺̜̻̞͈̳̫̎͌͋̉̊͌ͅͅr̴̨̢̢̢̡̨̧̧̛̰͔̭̮͔̟̙̖͖͙̣̠̫̜͔̩͍̜͇̣̤̝̟͚̰̱̝̳͍̜͈̭̜͈̼̮̤̻̩̹̈́͂̇̈́͑͊̈́̆̿̐̍͗̀̆́͂̇̓͛̿̒̽̑̃̀̈̂̆̓̽̑̓͐̔̽̓̈́͂͆̏̃̅̍̏̈́̓̍͗̄͐̑͆̎̇͗͛̽̃͂͗̈̿̎̊̊̽̂̍̆̈́͒̀̈́̕͘͘͘̚͜͠͝͝͠͝͝͝͝ͅͅͅͅ ̸̢̢̡̨̨̧̧̨̨̡̛͓͇̭̲̪͎̯̯̙̻͚̦̤̣͉̝͓͎̯͕̹̬̖̩͔̥̦̰̫̰͈̗͔͍̼̩͚̲̳̹̩̜̩̰̬̣̦̗̞̩̮̣̪̜̘͇͈̪̭̫̥̠̘̣̩̞̟͙͖̻͓̱̘̜̮̗̻̮̝̙̘̹̬̜͉̱͙̲͖̭̩͚̺͂̌͑̊̽̋̄̈́̄̌̍͐̑͊̓̾͒̉̔̋̀̂̂̓̇́͆̑̆̔͊͑̓̎̐͗̈͒̐̈́̇͘͘͜͜͝͠͝͝ͅͅͅȁ̷̛̤̩̻͍̩̾̂̆̎̿̌̉̓̄̂͆͠ͅṃ̸̧̧̧̢̨̢̧̛͍͕̣̞͓̱̯͇͉͓͈͕̣͕̰͇̠̲̲̠͙̯̥̭͚̻͙̪̣͉̳̯͚͔̦͉̞̱̼̳̰̘͖̻͎̝̰͎̳̩̓͐͊̊͒̉̇́͑͒͂̅̓̊͋̔͗̾̾͊̂͊̈́́͑͂̄͊́̄̆̌̏̒̓̑̀̏̽͌̊̐̆̅̽̈́̂̒̏̂̏̀̐̏͌̃́̆̋̈́̈̋̕͘̕͘̕̕̚̚̚͜͜͜͠͝͠͝͠͝ ̴̧̡̧̡̨̡̡̢̨̢̡̢̢̧̢̛͖̭̰̲̻̬͕̞̯͎͔̣̫̱͉̳͇̬͔̱̙̼̝͚̭̘͙̫̝̰̼̳͕̲͙̞̥̟̗̮̩͓̣̮̙̱̙̻̞͓̹̱̫͍͇̺̯̜̣̙̖͍̬̟͖̺̦̞̮̠̞͙̯͇͎͙͎̺̣̲̙̼̲̙̣̳͛͆̌̈́̅͐͌̀̂́͑̈́͑̽̑̃̈́̋̓͐͂́̓̃̿̏̎̈͊̀̒̇͋͗̋͒͛́̑̆̓̀̇̓̑̊̈̈́́̊̀̀͗̇̐̑̆̐̂̂̂̈́͋̎̀̋̅͒͛̾̀̈́̂̉̀̒̀̅͛̓̆̐̔̅͋̓͒̋̄̕̕̕͜͜͠͝͠͝͝͝͠ͅͅͅį̸̡̳͕͓̖̭͔̳͉̯͓͍͚͕̱͓̞̝͍̙̠̣̞͎̞̣̪͉̿͑̆̅̄͐͌̂̓͐̍̆̃̔̎͒͊̎̈́̿̒͛̉͐́̓̉͌͂̐̈́̅̂̐̽̑̽̆̿̒͊͂̈́̈́͑̓͘̕͠͠͝͝͠");

            comparer = new CompareProjectionStates();
        };

        static Try<bool> result;

        Because of = () => result = comparer.TryCheckEquality(left, right);

        It should_fail = () => result.Success.ShouldBeFalse();
        It should_have_an_exception = () => result.HasException.ShouldBeTrue();
        It should_return_false = () => result.Result.ShouldBeFalse();
    }
}
