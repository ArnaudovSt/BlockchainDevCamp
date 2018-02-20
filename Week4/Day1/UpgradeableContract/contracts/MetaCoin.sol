pragma solidity ^0.4.17;

import "./ConvertLib.sol";
import "./MetaCoinStorage.sol";

// This is just a simple example of a coin-like contract.
// It is not standards compatible and cannot be expected to talk to other
// coin/token contracts. If you want to create a standards-compliant
// token, see: https://github.com/ConsenSys/Tokens. Cheers!

contract MetaCoin {
	event Transfer(address indexed _from, address indexed _to, uint256 _value);

	MetaCoinStorage db;
	function MetaCoin(address _storageAddress) public {
		require(_storageAddress != address(0));
		db = MetaCoinStorage(_storageAddress); 
	}

	function sendCoin(address receiver, uint amount) public returns(bool sufficient) {
		if (db.balances(msg.sender) < amount) {
			return false;	
		} 

		db.setBalance(msg.sender, db.balances(msg.sender) - amount);
		db.setBalance(receiver, db.balances(receiver) + amount);

		Transfer(msg.sender, receiver, amount);
		return true;
	}

	function getBalanceInEth(address addr) public view returns(uint) {
		return ConvertLib.convert(getBalance(addr),2);
	}

	function getBalance(address addr) public view returns(uint) {
		return db.balances(addr);
	}
}