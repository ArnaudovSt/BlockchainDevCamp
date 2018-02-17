pragma solidity ^0.4.18;

import './common/SafeMath.sol';
import './Withdrawable.sol';

contract CompanyShares is Withdrawable {
    using SafeMath for uint256;
    
    struct ShareholderInfo {
        uint256 totalShares;
        uint256 sharesAllowedToSell;
    }

    mapping(address => ShareholderInfo) shareholders;

    uint256 public totalSupply;
    uint256 public price;
    uint256 public dividentPerShare;
    

    event LogPurchase(address indexed _buyer, uint256 _amount);
    event LogSale(address indexed _seller, uint256 _amount);

    function CompanyShares(uint256 _totalSupply, uint256 _priceInWei, uint256 _dividentPerShare) public {
        totalSupply = _totalSupply;
        price = _priceInWei;
        dividentPerShare = _dividentPerShare;
    }

    function buy() public payable {
        uint256 sellAmount = msg.value.mul(1 ether);
        sellAmount = sellAmount.div(price);

        require(totalSupply >= sellAmount);
        totalSupply = totalSupply.sub(sellAmount);

        shareholders[msg.sender].totalShares = shareholders[msg.sender].totalShares.add(sellAmount);

        LogPurchase(msg.sender, sellAmount);
    }

    function sell(uint256 _sharesAmount) public {
        require(shareholders[msg.sender].sharesAllowedToSell >= _sharesAmount);

        uint256 buyAmount = _sharesAmount.mul(price.add(dividentPerShare));
        buyAmount = buyAmount.div(1 ether);
        require(this.balance >= buyAmount);

        totalSupply = totalSupply.add(_sharesAmount);

        ShareholderInfo memory currentInfo = shareholders[msg.sender];
        currentInfo.totalShares = currentInfo.totalShares.sub(_sharesAmount);
        currentInfo.sharesAllowedToSell = currentInfo.sharesAllowedToSell.sub(_sharesAmount);
        shareholders[msg.sender] = currentInfo;

        msg.sender.transfer(buyAmount);

        LogSale(msg.sender, _sharesAmount);
    }

    function allowSpending(address _shareholder, uint256 _amount) public onlyOwner {
        require(_shareholder != address(0));
        shareholders[_shareholder].sharesAllowedToSell = _amount;
    }
}