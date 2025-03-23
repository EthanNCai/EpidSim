using System.ComponentModel;
public class CFECommonFees<TPlace> : CFECommon where TPlace : Place, IContributablePlace
{
    public TPlace relatedPlace;
    //  CFECommonTax继承 CFECommon， 所以，在这里声明的一切属性都是只服务于Tax的，也就是这里的RelatedPlace
    //  包括一个overridce的update ContributeItem 的函数（此函数依赖于Tax的使用目的，因此需要在
    // 这里而不是 父对象里面声明。）
    public CFECommonFees(TPlace place):
        base(place.placeFullName,new ContributeItem(ContributeSubTypes.Fees)){
        this.relatedPlace = place;
    }
    public override void QUpdateContributeItem(){
        int newQcontribution = this.relatedPlace.CalculateQContribution();
        base.contributeItem.QUpdateContribution(newQcontribution);
    }  

}

