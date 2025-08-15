using System.Collections.Generic;
using GKAPI.Difficulties;
using GKAPI.Items;
using I2.Loc;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace GKAPI.Lang;

public class LangAPI
{
    private List<TermData> _terms = [];
    
    public static LangAPI Instance { get; } = new();

    private LangAPI()
    {
        EventHandler.LateInit += RegisterTerms;
    }

    public TermData CreateTerm(string key, string value)
    {
        var languages = new Il2CppStringArray([value, value, value, value, value, value, value, value, value, value, value, value, value]);
        var flags = new Il2CppStructArray<byte>([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
        return new TermData
        {
            Term = key,
            Description = "",
            TermType = eTermType.Text,
            Languages = languages,
            Flags = flags
        };
    }

    public void CreateAndAddTerm(string key, string value)
    {
        if (key is null or "") return;
        AddTerm(CreateTerm(key, value));
    }

    public void AddTerm(TermData term)
    {
        _terms.Add(term);
    }

    public void AddItemLang(GkItem item)
    {
        var info = item.Info;
        CreateAndAddTerm(info.itemNameKey.mTerm, item.Name);
        CreateAndAddTerm(info.itemLongDescKey.mTerm, item.Description);
        CreateAndAddTerm(info.itemStatsKey.mTerm, item.StatsDescription);
    }

    public void AddDifficultyLang(GkDifficulty difficulty)
    {
        CreateAndAddTerm("MENU.UI.DIFFICULTY." + difficulty.TranslationKey, $"{difficulty.DifficultyName}: <b>{difficulty.DifficultyValues.stringPercentage}</b>");
    }

    private void RegisterTerms()
    {
        foreach (var term in _terms)
        {
            LocalizationManager.Sources.get_Item(0).mTerms.Add(term);
        }
        //Seems to call an update function. Without this the terms don't get updated
        LocalizationManager.GetTermsList();
    }
}