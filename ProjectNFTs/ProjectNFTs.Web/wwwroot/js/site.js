// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


cPrev = -1; // global var saves the previous c, used to
// determine if the same column is clicked again

function sortBy(c) {
    rows = document.getElementById("sortable").rows.length; // num of rows
    columns = document.getElementById("sortable").rows[0].cells.length; // num of columns
    arrTable = [...Array(rows)].map(e => Array(columns)); // create an empty 2d array

    for (ro = 0; ro < rows; ro++) { // cycle through rows
        for (co = 0; co < columns; co++) { // cycle through columns
            // assign the value in each row-column to a 2d array by row-column
            arrTable[ro][co] = document.getElementById("sortable").rows[ro].cells[co].innerHTML;
        }
    }

    th = arrTable.shift(); // remove the header row from the array, and save it

    if (c !== cPrev) { // different column is clicked, so sort by the new column
        arrTable.sort(
             function (a, b) {
                // Convert string values to numbers for comparison
                var numA = parseFloat(a[c]);
                var numB = parseFloat(b[c]);

                // Check if the values are numbers
                if (!isNaN(numA) && !isNaN(numB)) {
                    return numA - numB; // Sort numerically
                } else {
                    return a[c].localeCompare(b[c]); // Sort alphabetically if not numbers
                }
            }
        );
    } else { // if the same column is clicked then reverse the array
        arrTable.reverse();
    }

    cPrev = c; // save in previous c

    arrTable.unshift(th); // put the header back in to the array

    // cycle through rows-columns placing values from the array back into the html table
    for (ro = 0; ro < rows; ro++) {
        for (co = 0; co < columns; co++) {
            document.getElementById("sortable").rows[ro].cells[co].innerHTML = arrTable[ro][co];
        }
    }

}

/*************************************************************************************************************/