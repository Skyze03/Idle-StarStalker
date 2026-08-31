using System;

[Serializable]
public class EquipmentInstance
{
    public string instanceId;
    public string templateId;
    public int level = 1;
    public bool locked;

    public EquipmentInstance(string template)
    {
        instanceId = Guid.NewGuid().ToString("N");
        templateId = template;
    }
}
