using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class WordSuggesterHelper : MonoBehaviour
{
    [SerializeField] private WordSuggesterAgent _agent;
    [SerializeField] private TMP_Text _text1;
    [SerializeField] private TMP_Text _text2;
    [SerializeField] private TMP_Text _text3;
    
    private Dictionary<Languages, String> langToStringName = new Dictionary<Languages, string>()
    {
        { Languages.en, "English" },
        { Languages.bg, "Bulgarian" },
        { Languages.es, "Spanish" },
        { Languages.fr, "France" },
        { Languages.hu, "Hungarian" }
    };
    
    async public void Populate(String wordInEnglish, Languages targetLanguage)
    {
        String languageString = langToStringName[targetLanguage];
        var relatedWords =  await 
            _agent.FindRelatedWords(wordInEnglish, 
                "English", languageString, 
                WordSuggesterAgent.WordCategory.Adjective);

        if (relatedWords.Count >= 1)
        {
            _text1.text = relatedWords[0].Item2;
        }
        if (relatedWords.Count >= 2)
        {
            _text2.text = relatedWords[1].Item2;
        }
        if (relatedWords.Count >= 3)
        {
            _text3.text = relatedWords[2].Item2;
        }
    }
}
