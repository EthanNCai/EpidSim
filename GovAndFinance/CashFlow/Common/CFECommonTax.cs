using System.ComponentModel;
public class CFECommonTax<TPlace> : CFECommon where TPlace : Place, ICommonTaxContributor
{
    public TPlace relatedPlace;
    //  CFECommonTax继承 CFECommon， 所以，在这里声明的一切属性都是只服务于Tax的，也就是这里的RelatedPlace
    //  包括一个overridce的update ContributeItem 的函数（此函数依赖于Tax的使用目的，因此需要在
    // 这里而不是 父对象里面声明。）
    public CFECommonTax(TPlace place):
        base(place.placeFullName,new ContributeItem(ContributeSubTypes.Taxes))
    {
        this.relatedPlace = place;
    }
    public override void QUpdateContributeItem(){
        int newQcontribution = this.relatedPlace.calculateQContribution();
        base.contributeItem.QUpdateContribution(newQcontribution);
    }
}

public interface ICommonTaxContributor
{
    public int calculateQContribution();
}