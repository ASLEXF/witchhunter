using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletinBoard : Interactive
{
    [SerializeField] string file;

    public override void Interacted()
    {
        talk();
    }

    private void talk()
    {
        DialogBox.Instance.LoadAndStartText(file);
    }
}
