const abi = [{ "constant": false, "inputs": [{ "name": "_newOwner", "type": "address" }], "name": "setOwner", "outputs": [], "payable": false, "stateMutability": "nonpayable", "type": "function" }, { "constant": true, "inputs": [], "name": "owner", "outputs": [{ "name": "", "type": "address" }], "payable": false, "stateMutability": "view", "type": "function" }, { "anonymous": false, "inputs": [{ "indexed": true, "name": "_oldOwner", "type": "address" }, { "indexed": false, "name": "_newOwner", "type": "address" }], "name": "LogOwnerChanged", "type": "event" }, { "constant": false, "inputs": [{ "name": "keccak", "type": "bytes32" }], "name": "add", "outputs": [{ "name": "dateAdded", "type": "uint256" }], "payable": false, "stateMutability": "nonpayable", "type": "function" }, { "constant": true, "inputs": [{ "name": "keccak", "type": "bytes32" }], "name": "verify", "outputs": [{ "name": "dateAdded", "type": "uint256" }], "payable": false, "stateMutability": "view", "type": "function" }];
const address = '0x87f7284485f33dbaf2c20f6dd905b8d9ad2abde9';

$(() => {
    $('#linkHome').click(function () {
        showView('viewHome')
    });
    $('#linkSubmitDocument').click(function () {
        showView('viewSubmitDocument')
    });
    $('#linkVerifyDocument').click(function () {
        showView('viewVerifyDocument')
    });
    $('#documentUploadButton').click(uploadDocument);
    $('#documentVerifyButton').click(verifyDocument);
    // Attach AJAX 'loading' event listener
    $(document).on({
        ajaxStart: function () {
            $('#loadingBox').show()
        },
        ajaxStop: function () {
            $('#loadingBox').hide()
        }
    });

    function showView(viewName) {
        // Hide all views and show the selected view only
        $('main > section').hide();
        $('#' + viewName).show();
    }

    function showInfo(message) {
        $('#infoBox>p').html(message);
        $('#infoBox').show();
        $('#infoBox>header').click(function () {
            $('#infoBox').hide();
        });
    }

    function showError(errorMsg) {
        $('#errorBox>p').html('Error: ' + errorMsg);
        $('#errorBox').show();
        $('#errorBox>header').click(function () {
            $('#errorBox').hide();
        });
    }

    function uploadDocument() {
        if ($('#documentForUpload')[0].files.length === 0) {
            return showError('Please select a file to upload.');
        }

        let fileReader = new FileReader();
        fileReader.onload = () => {
            let documentHash = sha256(fileReader.result);
            if (!web3) {
                return showError('Please install MetaMask.');
            }

            let contract = web3.eth.contract(abi).at(address);

            contract.add(documentHash, (err, result) => {
                if (err) {
                    return showError(`Smart contract call failed: ${err}`);
                }

                showInfo(`Document ${documentHash} <b>successfully added</b> to the registry`);
            });
        }
        fileReader.readAsBinaryString($('#documentForUpload')[0].files[0]);
    }

    function verifyDocument() {
        if ($('#documentToVerify')[0].files.length == 0)
            return showError("Please select a file to verify.");
        let fileReader = new FileReader();
        fileReader.onload = () => {
            let documentHash = sha256(fileReader.result);
            if (typeof web3 === 'undefined') {
                return showError("Please install MetaMask.");
            }
            let contract = web3.eth.contract(abi).at(address);
            contract.verify(documentHash, function (err, result) {
                if (err) {
                    return showError("Smart contract call failed: " + e);
                }
                let contractPublishDate = result.c;
                if (contractPublishDate > 0) {
                    let displayDate = new Date(contractPublishDate * 1000).toLocaleString();
                    showInfo(`Document ${documentHash} is <b>valid<b>, date published: ${displayDate}`);
                } else {
                    return showError(`Document ${documentHash} is <b>invalid</b>: not found in the registry.`);
                }
            });
        }
        fileReader.readAsBinaryString($('#documentToVerify')[0].files[0]);
    }
});